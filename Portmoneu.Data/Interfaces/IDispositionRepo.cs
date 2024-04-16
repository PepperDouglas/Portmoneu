using Portmoneu.Models.Entities;

namespace Portmoneu.Data.Interfaces
{
    public interface IDispositionRepo
    {
        Task RecordDispositionRelation(Disposition disposition);
    }
}
