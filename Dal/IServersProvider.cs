using System.Collections.Generic;
using System.Threading.Tasks;
using Domain;


namespace Dal
{
    public interface IServersProvider
    {
        event UpdateHandler OnUpdateServers;

        Task<long> DeleteUnactiveServers();
        Task<List<ServerEntity>> LoadServers();
        string SelectServerForPlayerJson(PlayerInfo playerInfo);
        void TestFillDB();
        Task<bool> UpdateServer(ServerInfo serverInfo);
    }


    public delegate void UpdateHandler(bool withClearing);
}