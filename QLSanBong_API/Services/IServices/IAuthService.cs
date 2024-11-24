using QLSanBong_API.Models;

namespace QLSanBong_API.Services.IService
{
    public interface IAuthService
    {
        //Action
        string AddAction(Models.Action action);
        List<Models.Action> GetAllAction();
        bool DeleteAction(string actionid);
        bool UpdateAction(string actionid, string newactionname);

        //Service
        string AddService(Models.Service service);
        List<Models.Service> GetAllService();
        bool DeleteService(string serviceid);
        bool UpdateService(string serviceid, string newservicename);

        string GetAuth(string serviceid, string actionid);
        List<Models.ActionService> GetAllActionService();
        void SyncActionService();
    }
}
