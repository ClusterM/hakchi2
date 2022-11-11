using System;
using System.IO;
using System.Reflection;

namespace com.clusterrr.hakchi_gui
{
    public class ApplicationDeployment
    {
        private static bool inited = false;
        private static Type applicationDeploymentType;

        public static bool IsNetworkDeployed
        {
            get
            {
                if (!inited) init();
                if (applicationDeploymentType == null) return false;

                ExternalPropertyHelper<bool> IsNetworkDeployedProp = new ExternalPropertyHelper<bool>(applicationDeploymentType.GetProperty("IsNetworkDeployed"));
                return IsNetworkDeployedProp.Value;
            }
        }

        public static ApplicationDeployment CurrentDeployment
        {
            get { return new ApplicationDeployment(); }
        }

        public bool IsFirstRun
        {
            get
            {
                if (applicationDeploymentType == null) return false;

                ExternalPropertyHelper<object> CurrentDeploymentProp = new ExternalPropertyHelper<object>(applicationDeploymentType.GetProperty("CurrentDeployment"));
                object CurrentDeployment = CurrentDeploymentProp.Value;
                return (bool)CurrentDeployment.GetType().GetProperty("IsFirstRun").GetValue(CurrentDeployment, null);
            }
        }

        private static void init()
        {
            try
            {
                string runtimeDirectory = System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory();
                Assembly systemDeploymentAssembly = Assembly.LoadFrom(Path.Combine(runtimeDirectory, "System.Deployment.dll"));
                applicationDeploymentType = systemDeploymentAssembly.GetType("System.Deployment.Application.ApplicationDeployment");
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine("Could not load System.Deployment.dll dynamically: {0}", e.Message);
            }
            finally
            {
                inited = true;
            }
        }

        private class ExternalPropertyHelper<PropertyType>
        {
            delegate PropertyType GetFunction();
            GetFunction GetValue;

            public ExternalPropertyHelper(PropertyInfo externalProperty)
            {
                MethodInfo getMethod = externalProperty.GetGetMethod();
                GetFunction getter = (GetFunction)Delegate.CreateDelegate(typeof(GetFunction), getMethod);
                GetValue = getter;
            }

            public PropertyType Value
            {
                get { return GetValue(); }
            }
        }
    }
}
