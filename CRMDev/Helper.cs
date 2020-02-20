using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk.Query;

namespace CRMDev
{
    class Helper
    {
        public  Entity getCategory(string categoryName , IOrganizationService service)
        {
            var Account_query = new QueryExpression("new_category") { ColumnSet = new ColumnSet(true) };//Select * from tbl_name
            #region Account Query
            Account_query.Criteria.AddCondition(new ConditionExpression("new_name", ConditionOperator.Equal, categoryName));
            Account_query.Criteria.FilterOperator = LogicalOperator.And;
            #endregion
            Entity category = service.RetrieveMultiple(Account_query).Entities[0];
            return category;
        }

        public Entity FindById(Guid entityID, IOrganizationService service)
        {
            //var projectQuery = new QueryExpression("new_project") { ColumnSet = new ColumnSet(true) };//Select * from tbl_name
            var project = service.Retrieve("lead", entityID, new ColumnSet(true));
            return project;
        }
    }
}
