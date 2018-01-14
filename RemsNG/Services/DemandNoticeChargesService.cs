using RemsNG.ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Services.Interfaces
{
    public class DemandNoticeChargesService : IDemandNoticeCharges
    {
        private ILcdaService lcdaService;
        public DemandNoticeChargesService(ILcdaService _lcdaService)
        {
            lcdaService = _lcdaService;
        }
        
        public async Task<decimal> getCharges(decimal amount, Guid lcdaId)
        {
            decimal finalcharges = 0;
            Lgda lcdaExtension = await lcdaService.Get(lcdaId);
            if (lcdaExtension != null)
            {
                if (lcdaExtension.charges == 0)
                {
                    //1%,200
                    if (amount <= 20000)
                    {
                        finalcharges = 200;
                    }
                    else
                    {
                        finalcharges = decimal.Round((1/100) * amount,2);
                    }
                }
                else
                {
                    decimal charges = (lcdaExtension.charges / 100) * amount;
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
