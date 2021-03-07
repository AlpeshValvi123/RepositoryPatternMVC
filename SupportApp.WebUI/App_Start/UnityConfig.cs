using SupportApp.BLL.Interfaces;
using SupportApp.BLL.Services;
using SupportApp.DAL.Repository;
using System.Web.Mvc;
using Unity;
using Unity.Mvc5;

namespace SupportApp.WebUI
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
			var container = new UnityContainer();

            // register all your components with the container here
            // it is NOT necessary to register your controllers

            container.RegisterType(typeof(IRepository<>), typeof(EntityRepository<>));

            container.RegisterType<IUserService, UserService>();

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}