using System.Collections.Generic;
using System.Threading.Tasks;
using Domain;


namespace Dal
{
    public interface IPlayersProvider
    {
        Task<bool> EditPlayer(int playerId, string parameterToUpdate, object newValue);
        Task<List<PlayerEntity>> LoadPlayers();
        void TestFillDB();
    }
}