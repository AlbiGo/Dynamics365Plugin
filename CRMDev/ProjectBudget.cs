using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRMDev
{
   
    public class ProjectBudget : IPlugin
    {
        public string getSelectedValue(int selectedValue)
        {
            string category = "";
            switch (selectedValue)
            {
                case 100000000:
                    category = "IT";
                    break;
                case 100000001:
                    category = "Marketing";
                    break;
                case 100000002:
                    category = "Finance";
                    break;
                case 100000003:
                    category = "Social";
                    break;
                case 100000004:
                    category = "Office Supply";
                    break;
                default:
                    break;

            }
            return category;
        }

        public void Execute(IServiceProvider serviceProvider)
        {

            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            IOrganizationServiceFactory factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));

            IOrganizationService service = factory.CreateOrganizationService(context.UserId);
            Helper helper = new Helper();


            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
            {
                Entity entity = (Entity)context.InputParameters["Target"];
                string message = context.MessageName;
                decimal projectBudget = (decimal)entity["new_projectbudget"]; //The Project Budget

                try
                {

                    if(message.Equals("Update"))
                    {
                        Entity preimage = context.PreEntityImages.Contains("Project") ? context.PreEntityImages["Project"] : null;
                        var selection = (OptionSetValue)preimage["new_category"];
                        int selectedValue = selection.Value;
                        string category = "";
                        category = this.getSelectedValue(selectedValue);
                        Entity categoryEntity = helper.getCategory(category, service);
                        decimal categoryBudget = (decimal)categoryEntity["new_categorybudget"];
                        if (projectBudget > categoryBudget)
                        {
                            throw new InvalidPluginExecutionException("The Entered Budget is bigger than the Category Budget" + System.Environment.NewLine + "The budget must be under : " + categoryBudget);
                        }


                    }
                    else if(message.Equals("Create"))
                    {
                        // GET 
                        if(entity.Contains("new_category"))
                        {
                            var selection = (OptionSetValue)entity["new_category"];
                            int selectedValue = selection.Value;
                            string category = "";
                            category = this.getSelectedValue(selectedValue);

                            Entity categoryEntity = helper.getCategory(category,service);
                            decimal categoryBudget = (decimal)categoryEntity["new_categorybudget"];
                            if(projectBudget > categoryBudget)
                            {
                                throw new InvalidPluginExecutionException("The Entered Budget is bigger than the Category Budget"+ System.Environment.NewLine + "The budget must be under : " + categoryBudget);
                            }
                        }
                       
                    }
                   
                }

                catch (Exception ex)
                {
                    throw new InvalidPluginExecutionException(ex.Message);
                }
            }
        }
    }
}
