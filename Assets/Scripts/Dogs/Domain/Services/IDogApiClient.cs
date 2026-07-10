using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Dogs.Domain.Models;

namespace Dogs.Domain.Services
{
    public interface IDogApiClient
    {
        UniTask<IReadOnlyList<BreedShortInfo>> GetBreedsAsync(int pageSize, CancellationToken cancellationToken);
        UniTask<BreedDetails> GetBreedDetailsAsync(string breedId, CancellationToken cancellationToken);
    }
}
