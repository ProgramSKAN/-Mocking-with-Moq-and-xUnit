using System;
using System.Collections.Generic;
using System.Text;

namespace CreditCardApplications
{
    public class FraudLookup
    {
        virtual public bool IsFraudRisk(CreditCardApplication application)//virtual >for partial mocks
        {
            if (application.LastName == "Smith")
            {
                return true;
            }

            return false;

            //return CheckApplication(application);//FOR CONCEPT OF MOCKING VIRTUAL PROTECTED MEMBER
        }

        protected virtual bool CheckApplication(CreditCardApplication application)
        {
            if (application.LastName == "Smith")
            {
                return true;
            }

            return false;
        }
    }
}
