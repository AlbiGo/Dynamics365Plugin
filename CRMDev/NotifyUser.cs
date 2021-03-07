using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRMDev
{
    public class NotifyUser : IPlugin // 
    {
        //public void getAdmin(IOrganizationService service)
        //{


        //    //Guid AdminRoleTemplateId = new Guid("1A968EDE-491E-EA11-A812-000D3ABA6D7C"); 

        //    //QueryExpression query = new QueryExpression("role");

        //    //query.Criteria.AddCondition("roletemplateid", ConditionOperator.Equal, AdminRoleTemplateId);

        //    //LinkEntity link = query.AddLink("systemuserroles", "roleid", "roleid");

        //    //link.LinkCriteria.AddCondition("systemuserid", ConditionOperator.Equal, AdminRoleTemplateId);

        //    // var results =  service.RetrieveMultiple(query).Entities;
           
        //    //return results;
        //}
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

                try
                {
                    WhoAmIRequest systemUserRequest = new WhoAmIRequest();
                    WhoAmIResponse systemUserResponse = (WhoAmIResponse)service.Execute(systemUserRequest);
                    Guid userId = systemUserResponse.UserId;
                    EntityReference owner = (EntityReference)entity.Attributes["ownerid"];
                    var user = helper.FindById(owner.Id, service, "systemuser");

                    #region Send Email to Request Handler
                    Entity fromParty = new Entity("activityparty");
                    fromParty["partyid"] = new EntityReference("systemuser", userId);
                    Entity toParty = new Entity("activityparty");
                    toParty["partyid"] = new EntityReference("systemuser", user.Id);

                    Entity Email = new Entity("email");
                    Email.Attributes["from"] = new Entity[] { fromParty };
                    Email.Attributes["to"] = new Entity[] { toParty };
                    Email.Attributes["subject"] = "You have a new Request";
                    Email.Attributes["regardingobjectid"] = new EntityReference("incident", entity.Id);
                    Email.Attributes["description"] = "Request Discription";
                    Email.Attributes["ownerid"] = new EntityReference("systemuser", userId);
                    Guid EmailId = service.Create(Email);

                    SendEmailRequest req = new SendEmailRequest();
                    req.EmailId = EmailId;
                    req.IssueSend = true;
                    req.TrackingToken = "";

                    SendEmailResponse res = (SendEmailResponse)service.Execute(req);

                    #endregion
                }
                catch (Exception ex)
                {
                    throw new InvalidPluginExecutionException(ex.Message);
                }
            }
        }
    }
}
