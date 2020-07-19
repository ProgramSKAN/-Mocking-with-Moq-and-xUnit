using System;
using System.Collections.Generic;
using System.Text;

namespace CreditCardApplications
{
    public enum CreditCardApplicationDecision
    {
        Unknown,
        AutoAccepted,
        AutoDeclined,
        ReferredToHuman,
        ReferredToHumanFraudRisk
    }
}
