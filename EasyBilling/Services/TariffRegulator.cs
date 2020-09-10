using EasyBilling.Data;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EasyBilling.Models.Pocos;

namespace EasyBilling.Services
{
    public class TariffRegulator
    {
        private BillingDbContext _dbContext;

        public TariffRegulator(IServiceScopeFactory scopeFactory)
        {
            var scope = scopeFactory.CreateScope();
            var sp = scope.ServiceProvider;

            _dbContext = sp.GetRequiredService<BillingDbContext>();
        }

        /// <summary>
        /// Провести автоматическую оплату по тарифу
        /// </summary>
        /// <param name="profile">Принимает экземпляр профиля</param>
        /// <returns></returns>
        private async Task PutOfCashAsync(Profile profile)
        {
            if (profile != null && profile.Tariff != null)
            {
                if (profile.AmountOfCash < profile.Tariff.Price)
                {
                    profile.IsHolded = true;
                }
                else
                {
                    using (var transaction = _dbContext.Database.BeginTransaction())
                    {
                        profile.AmountOfCash -= profile.Tariff.Price;
                        await _dbContext.Payments.AddAsync(new Models.Pocos.Payment()
                        {
                            DestinationProfile = profile,
                            Comment = "Автоматический вычет тарифа",
                            Amount = -profile.Tariff.Price
                        });
                        profile.DateBeginOfUseOfTarrif = DateTime.Now;

                        transaction.Commit();
                    }
                }
            }
        }

        /// <summary>
        /// Проверка срока действия тарифа
        /// </summary>
        /// <returns></returns>
        public async Task CheckProfilesForTariffExpiringAsync()
        {
            using (_dbContext)
            {
                var iQuery = _dbContext.Profiles
                    .Include(p => p.Tariff)
                    .Where(p => p.Tariff.AmounfOfDays != 0 &&
                        !p.IsHolded && !p.IsEnabled);

                var currDate = DateTime.Now;
                await iQuery.ForEachAsync(p =>
                {
                    try
                    {
                        var expiryDate = p.DateBeginOfUseOfTarrif
                            .AddDays(p.Tariff.AmounfOfDays);
                        if (currDate >= expiryDate)
                        {
                            PutOfCashAsync(p).Wait();
                        }
                        _dbContext.Update(p);
                    }
                    catch (Exception)
                    { }
                });

                await _dbContext.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Вычет суммы за тариф
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task StartToUseOfTariffAsync(int id)
        {
            using (_dbContext)
            {
                var profile = await _dbContext.Profiles
                    .FirstOrDefaultAsync(p => p.Id.Equals(id));

                if (profile != null &&
                    !profile.IsHolded && !profile.IsEnabled &&
                    profile.Tariff.AmounfOfDays != 0)
                {
                    var currDate = DateTime.Now;
                    var expiryDate = profile.DateBeginOfUseOfTarrif
                        .AddDays(profile.Tariff.AmounfOfDays);

                    if (currDate >= expiryDate)
                    {
                        using (_dbContext)
                        {
                            await PutOfCashAsync(profile);
                            _dbContext.Update(profile);
                            await _dbContext.SaveChangesAsync();
                        }
                    }
                }
            }
        }
    }
}
