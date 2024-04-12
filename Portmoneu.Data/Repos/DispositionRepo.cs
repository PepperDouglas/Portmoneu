using Portmoneu.Data.Contexts;
using Portmoneu.Data.Interfaces;
using Portmoneu.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portmoneu.Data.Repos
{
    public class DispositionRepo : IDispositionRepo
    {
        private readonly BankAppData _bankAppData;

        public DispositionRepo(BankAppData bankAppData) {
            _bankAppData = bankAppData;
        }

        public async Task RecordDispositionRelation(Disposition disposition) {
            await _bankAppData.Dispositions.AddAsync(disposition);
            await _bankAppData.SaveChangesAsync();
        }
    }
}
