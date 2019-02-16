using RemsNG.Common.Interfaces.Managers;
using RemsNG.Common.Models;
using System;
using System.Threading.Tasks;

namespace RemsNG.Infrastructure.Managers
{
    public class DemandNoticeChargesManagers : IDemandNoticeChargesManagers
    {
        private ILcdaManagers lcdaService;
        public DemandNoticeChargesManagers(ILcdaManagers _lcdaService)
        {
            lcdaService = _lcdaService;
        }

        public async Task<decimal> getCharges(decimal amount, Guid lcdaId)
        {
            decimal finalcharges = 0;
            LcdaModel lcdaExtension = await lcdaService.Get(lcdaId);
            if (lcdaExtension != null)
            {
                if (lcdaExtension.Charges == 0)
                {
                    //1%,200
                    if (amount <= 20000)
                    {
                        finalcharges = 200;
                    }
                    else
                    {
                        finalcharges = decimal.Round((1 / 100) * amount, 2);
                    }
                }
                else
                {
                    decimal charges = (lcdaExtension.Charges / 100) * amount;
                    if (amount <= 20000)
                    {
                        if (charges > 200)
                        {
                            finalcharges = charges;
                        }
                        else
                        {
                            finalcharges = 200;
                        }
                    }
                    else
                    {
                        finalcharges = charges;
                    }
                }
            }
            else
            {
                if (amount <= 20000)
                {
                    finalcharges = 200;
                }
                else
                {
                    finalcharges = (1 / 100) * amount;
                }
            }

            return decimal.Round(finalcharges, 2);
        }
    }
}
