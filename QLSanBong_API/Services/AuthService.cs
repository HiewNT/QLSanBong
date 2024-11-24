using QLSanBong_API.Data;
using QLSanBong_API.Models;
using QLSanBong_API.Services.IService;
using Microsoft.Extensions.Logging;
using System;

namespace QLSanBong_API.Services
{
    public class AuthService : IAuthService
    {
        private readonly QlsanBongContext _context;
        private readonly ILogger<AuthService> _logger;

        public AuthService(QlsanBongContext context, ILogger<AuthService> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Action
        // Thêm mới Action
        public string AddAction(Models.Action action)
        {
            if (_context.Actions.Any(a => a.ActionId == action.ActionId))
                throw new InvalidOperationException("Action đã tồn tại.");

            // Map từ Models.Action sang Data.Action
            var dbAction = new Data.Action
            {
                ActionId = action.ActionId,
                ActionName = action.ActionName
            };

            _context.Actions.Add(dbAction);
            _context.SaveChanges();

            try
            {
                // Đồng bộ ActionService
                SyncActionService();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Lỗi đồng bộ sau khi thêm Action: {ex.Message}");
                throw new Exception("Đã thêm Action nhưng lỗi xảy ra trong quá trình đồng bộ.");
            }

            return dbAction.ActionId;
        }

        public List<Models.Action> GetAllAction()
        {
            // Map từ Data.Action sang Models.Action
            return _context.Actions.Select(a => new Models.Action
            {
                ActionId = a.ActionId,
                ActionName = a.ActionName
            }).ToList();
        }

        public bool DeleteAction(string actionId)
        {
            var dbAction = _context.Actions.FirstOrDefault(a => a.ActionId == actionId);
            if (dbAction == null)
                throw new KeyNotFoundException("Action not found.");

            _context.Actions.Remove(dbAction);
            _context.SaveChanges();
            return true;
        }

        public bool UpdateAction(string actionId, string newActionName)
        {
            var dbAction = _context.Actions.FirstOrDefault(a => a.ActionId == actionId);
            if (dbAction == null)
                throw new KeyNotFoundException("Action not found.");

            dbAction.ActionName = newActionName;
            _context.SaveChanges();
            return true;
        }

        // Service
        // Thêm mới Service
        public string AddService(Models.Service service)
        {
            if (_context.Services.Any(s => s.ServiceId == service.ServiceId))
                throw new InvalidOperationException("Service đã tồn tại.");

            // Map từ Models.Service sang Data.Service
            var dbService = new Data.Service
            {
                ServiceId = service.ServiceId,
                ServiceName = service.ServiceName
            };

            _context.Services.Add(dbService);
            _context.SaveChanges();

            try
            {
                // Đồng bộ ActionService
                SyncActionService();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Lỗi đồng bộ sau khi thêm Service: {ex.Message}");
                throw new Exception("Đã thêm Service nhưng lỗi xảy ra trong quá trình đồng bộ.");
            }

            return dbService.ServiceId;
        }

        public List<Models.Service> GetAllService()
        {
            // Map từ Data.Service sang Models.Service
            return _context.Services.Select(s => new Models.Service
            {
                ServiceId = s.ServiceId,
                ServiceName = s.ServiceName
            }).ToList();
        }

        public bool DeleteService(string serviceId)
        {
            var dbService = _context.Services.FirstOrDefault(s => s.ServiceId == serviceId);
            if (dbService == null)
                throw new KeyNotFoundException("Service not found.");

            _context.Services.Remove(dbService);
            _context.SaveChanges();
            return true;
        }

        public bool UpdateService(string serviceId, string newServiceName)
        {
            var dbService = _context.Services.FirstOrDefault(s => s.ServiceId == serviceId);
            if (dbService == null)
                throw new KeyNotFoundException("Service not found.");

            dbService.ServiceName = newServiceName;
            _context.SaveChanges();
            return true;
        }

        // Phương thức lấy authId từ serviceId và actionId
        public string GetAuth(string serviceId, string actionId)
        {
            // Truy vấn ActionService để tìm authId dựa trên serviceId và actionId
            var auth = _context.ActionServices
                .Where(auth => auth.ServiceId == serviceId && auth.ActionId == actionId) // So sánh đúng với các thuộc tính của đối tượng
                .FirstOrDefault(); // Lấy bản ghi đầu tiên phù hợp

            if (auth != null)
            {
                return auth.Id; // Trả về authId
            }

            return null; // Nếu không tìm thấy, trả về null
        }



        //ActionService
        public List<Models.ActionService> GetAllActionService()
        {
            // Map từ Data.Action sang Models.Action
            return _context.ActionServices.Select(a => new Models.ActionService
            {
                ID=a.Id,
                ActionId = a.ActionId,
                ServiceId = a.ServiceId
            }).ToList();
        }

        public void SyncActionService()
        {
            try
            {
                var actions = _context.Actions.ToList();
                var services = _context.Services.ToList();

                var newActionServices = actions
                    .SelectMany(action => services, (action, service) => new Data.ActionService
                    {
                        Id = $"{service.ServiceId}_{action.ActionId}",
                        ActionId = action.ActionId,
                        ServiceId = service.ServiceId
                    }).ToList();

                _context.ActionServices.RemoveRange(_context.ActionServices);
                _context.ActionServices.AddRange(newActionServices);
                _context.SaveChanges();

                _logger.LogInformation("Đồng bộ bảng ActionService thành công.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Lỗi trong quá trình đồng bộ: {ex.Message}");
                throw new Exception("Đã xảy ra lỗi khi đồng bộ dữ liệu.");
            }
        }
    }
}
