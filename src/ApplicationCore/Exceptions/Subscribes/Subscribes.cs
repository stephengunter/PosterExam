using System;
using System.Collections.Generic;
using System.Text;
using ApplicationCore.Models;

namespace ApplicationCore.Exceptions
{
    public class CreateWhileCurrentSubscribeExist : Exception
    {
        //目前仍在訂閱期, 試圖建立新訂閱
        public CreateWhileCurrentSubscribeExist(string userId) : base($"userId: {userId}")
        {

        }
    }

    public class SelectedPlanDifferentFromActivePlan : Exception
    {
        //試圖建立新訂閱時, 找不到指定的方案
        public SelectedPlanDifferentFromActivePlan(int selectedPlanId, int activePlanId) : base($"selectedPlanId: {selectedPlanId}  , activePlanId: {activePlanId}")
        {

        }
    }


    public class SelectedPriceDifferentFromActivePlan : Exception
    {
        //指定的方案價格與ActivePlan不同
        public SelectedPriceDifferentFromActivePlan(int selectedPlanId, int activePlanId) : base($"selectedPlanId: {selectedPlanId}  , activePlanId: {activePlanId}")
        {

        }
    }

}
