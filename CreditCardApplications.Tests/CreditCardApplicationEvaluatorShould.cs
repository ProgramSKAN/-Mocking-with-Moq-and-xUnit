using Moq;
using Moq.Protected;
using System;
using System.Collections.Generic;
using Xunit;

namespace CreditCardApplications.Tests
{
    public class CreditCardApplicationEvaluatorShould
    {
        /*//------REDUCE CODE DUPLICATON USING THIS, AND USE IT IN EVERY TEST IN THIS CLASS---------
         
        private Mock<IFrequentFlyerNumberValidator> mockValidator;
        private CreditCardApplicationEvaluator _sut;

        public CreditCardApplicationEvaluatorShould()
        {
            mockValidator = new Mock<IFrequentFlyerNumberValidator>();
            mockValidator.SetupAllProperties();
            mockValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");
            mockValidator.Setup(x => x.IsValid(It.IsAny<string>())).Returns(true);

            _sut = new CreditCardApplicationEvaluator(mockValidator.Object);
        }
        //----------------------------------------------------------------------------------------*/

        [Fact]
        public void AcceptHighIncomeApplications()
        {
            Mock<IFrequentFlyerNumberValidator> mockValidator = new Mock<IFrequentFlyerNumberValidator>();
            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);//pass mockValidator instead of null.since we not implemented interface.

            var application = new CreditCardApplication { GrossAnnualIncome = 100_000 };

            CreditCardApplicationDecision decision = sut.Evaluate(application);

            Assert.Equal(CreditCardApplicationDecision.AutoAccepted, decision);
        }

        [Fact]
        public void ReferYoungApplications()
        {
            var mockValidator = new Mock<IFrequentFlyerNumberValidator>();
            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            mockValidator.DefaultValue = DefaultValue.Mock;//SPECIFYING DEFAULLT VALUE BEHAVIOUR FOR LOOSE MOCKS.we added this after we have a hierarchy in interface properties

            var application = new CreditCardApplication { Age = 19 };

            CreditCardApplicationDecision decision = sut.Evaluate(application);

            Assert.Equal(CreditCardApplicationDecision.ReferredToHuman, decision);
        }

        [Fact]
        public void DeclineLowIncomeApplications()
        {
            Mock<IFrequentFlyerNumberValidator> mockValidator =new Mock<IFrequentFlyerNumberValidator>();

            //mockValidator.Setup(x => x.IsValid("y")).Returns(true);//returns true only when FrequentFlyerNumber = "x"
            //mockValidator.Setup(x => x.IsValid(It.IsAny<string>())).Returns(true);//returns true regardless of FrequentFlyerNumber value
            //mockValidator.Setup(x => x.IsValid(It.Is<string>(number=>number.StartsWith("x")))).Returns(true);//returns true only if FrequentFlyerNumber value starts with "x"
            //mockValidator.Setup(x => x.IsValid(It.IsInRange<string>("a","z",Moq.Range.Inclusive))).Returns(true);//returns true only if FrequentFlyerNumber value is between "a" and "z" inclusive
            //mockValidator.Setup(x => x.IsValid(It.IsInRange("a", "z", Moq.Range.Inclusive))).Returns(true);//mentioning type is not necessary, it can automatically infer
            //mockValidator.Setup(x => x.IsValid(It.IsIn("z","y","x"))).Returns(true);//returns true only if FrequentFlyerNumber value is "z" or "y" or "x"
            mockValidator.Setup(x => x.IsValid(It.IsRegex("[a-z]"))).Returns(true);//returns true only if FrequentFlyerNumber value is in letters a to z


            mockValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");//this code placed after hierarchy licensekey

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication
            {
                GrossAnnualIncome = 19_999,
                Age = 42,
                FrequentFlyerNumber = "x"
            };

            CreditCardApplicationDecision decision = sut.Evaluate(application);

            Assert.Equal(CreditCardApplicationDecision.AutoDeclined, decision);

            //MockBehaviour.Strict> throws exception if mocked method is called but not been setup
            //MockBehaviour.Loose> never throws exception if mocked method is called but not been setup.it returns default values for value types,null for reference types,empty for array/enumerable
            //MockBehaviour.Default> Default Behaviour if none is specified.(MockBehaviour.Loose)
        }

        [Fact]
        public void ReferInvalidFrequentFlyerApplications()//STRICT MOCK
        {
            //Mock<IFrequentFlyerNumberValidator> mockValidator =new Mock<IFrequentFlyerNumberValidator>(MockBehavior.Strict);
            Mock<IFrequentFlyerNumberValidator> mockValidator = new Mock<IFrequentFlyerNumberValidator>();//loose mocking sicnce we added a new property Licencekey.so we want return value of it to be default.so that test won't break

            mockValidator.Setup(x => x.IsValid(It.IsAny<string>())).Returns(false);//even if the mock is loose,it will return false as a default.since now mock is strict we have to mention setup

            mockValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");//this code placed after hierarchy licensekey

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication();

            CreditCardApplicationDecision decision = sut.Evaluate(application);

            Assert.Equal(CreditCardApplicationDecision.ReferredToHuman, decision);
        }

        [Fact]
        public void ReferYoungApplications1()//above ReferYoungApplications test won't hit age block of code,so setup is needed
        {
            var mockValidator = new Mock<IFrequentFlyerNumberValidator>();

            mockValidator.DefaultValue = DefaultValue.Mock;//SPECIFYING DEFAULLT VALUE BEHAVIOUR FOR LOOSE MOCKS.we added this after we have a hierarchy in interface properties

            mockValidator.Setup(x => x.IsValid(It.IsAny<string>())).Returns(true);

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication { Age = 19 };

            CreditCardApplicationDecision decision = sut.Evaluate(application);

            Assert.Equal(CreditCardApplicationDecision.ReferredToHuman, decision);
        }

       /* [Fact]
        public void DeclineLowIncomeApplicationsOutDemo()//MOCKING METHODS WITH OUT PARAMETERS
        {
            Mock<IFrequentFlyerNumberValidator> mockValidator =new Mock<IFrequentFlyerNumberValidator>();

            bool isValid = true;//mock out param value
            mockValidator.Setup(x => x.IsValid(It.IsAny<string>(), out isValid));

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication
            {
                GrossAnnualIncome = 19_999,
                Age = 42
            };

            CreditCardApplicationDecision decision = sut.EvaluateUsingOut(application);

            Assert.Equal(CreditCardApplicationDecision.AutoDeclined, decision);
        }*/

        [Fact]
        public void ReferWhenLicenseKeyExpired()//CONFIGURING MOCKED PROPERTY TO RETURN A SPECIFIED VALUE
        {
            //var mockValidator = new Mock<IFrequentFlyerNumberValidator>();

            //mockValidator.Setup(x => x.IsValid(It.IsAny<string>())).Returns(true);

            //mockValidator.Setup(x => x.LicenseKey).Returns("EXPIRED"); OR
            //mockValidator.Setup(x => x.LicenseKey).Returns(GetLicenseKeyExpiryString);//GETTING A RETURN VALUE FROM A FUNCTION
            //-------------------AUTOMOCKING PROPERTY HIERARCHIES----------------------
            //var mockLicenseData = new Mock<ILicenseData>();
            //mockLicenseData.Setup(x=>x.LicenseKey).Returns("EXPIRED");
            //var mockServiceInfo = new Mock<IServiceInformation>();
            //mockServiceInfo.Setup(x => x.License).Returns(mockLicenseData.Object);
            //var mockValidator = new Mock<IFrequentFlyerNumberValidator>();
            //mockValidator.Setup(x => x.ServiceInformation).Returns(mockServiceInfo.Object);
            //OR
            var mockValidator = new Mock<IFrequentFlyerNumberValidator>();
            mockValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("EXPIRED");
            //--------------------------------------------------------------------------


            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication { Age = 42 };

            CreditCardApplicationDecision decision = sut.Evaluate(application);

            Assert.Equal(CreditCardApplicationDecision.ReferredToHuman, decision);
        }
        string GetLicenseKeyExpiryString()
        {
            //Ex. read from  vendor supplied constans file
            return "EXPIRED";
        }

        [Fact]
        public void UseDetailedLookupForOlderApplications()//TRACKING CHANGES TO MOCK PROPERTY VALUES
        {
            var mockValidator = new Mock<IFrequentFlyerNumberValidator>();

            //mockValidator.SetupProperty(x => x.ValidationMode);//By default mock related properties don't remember changes made to them.to make it remember setupproperty
            mockValidator.SetupAllProperties();//enebles change tracking for all of the objects.it must be placed before setup() code line like below
            mockValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");



            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication { Age = 30 };

            sut.Evaluate(application);

            Assert.Equal(ValidationMode.Detailed, mockValidator.Object.ValidationMode);
        }

        //----------BEHAVIOUR VERIFICATION TESTS--------------------------------------------
        [Fact]
        public void ValidateFrequentFlyerNumberForLowIncomeApplications()//VERIFY A METHOD WAS CALLED
        {
            var mockValidator = new Mock<IFrequentFlyerNumberValidator>();

            mockValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication();
            
            sut.Evaluate(application);

            //check whether CreditCardApplicationEvaluator is called IsValid with a value of null on mock IFrequentFlyerNumberValidator
            mockValidator.Verify(x => x.IsValid(null));
        }
        [Fact]
        public void ValidateFrequentFlyerNumberForLowIncomeApplications1()//VERIFY METHOD CALLED SPECIFIC NUMBER OF TIMES
        {
            var mockValidator = new Mock<IFrequentFlyerNumberValidator>();

            mockValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication
            {
                FrequentFlyerNumber = "q"
            };

            sut.Evaluate(application);

            //check whether CreditCardApplicationEvaluator is called IsValid with Any value on mock IFrequentFlyerNumberValidator one time
            mockValidator.Verify(x => x.IsValid(It.IsAny<string>()), Times.Once);
        }
        [Fact]
        public void ValidateFrequentFlyerNumberForLowIncomeApplications2()//ADDING A CUSTOM ERROR MESSAGE
        {
            var mockValidator = new Mock<IFrequentFlyerNumberValidator>();

            mockValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication
            {
                FrequentFlyerNumber = "q"
            };

            sut.Evaluate(application);

            mockValidator.Verify(x => x.IsValid(It.IsAny<string>()), "Frequent flyer numbers should be validated");//custom error message if not called
        }

        [Fact]
        public void NotValidateFrequentFlyerNumberForHighIncomeApplications()//VERIFY METHOD WAS NOT CALLED 
        {
            var mockValidator = new Mock<IFrequentFlyerNumberValidator>();

            mockValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication { GrossAnnualIncome = 100_000 };

            sut.Evaluate(application);

            mockValidator.Verify(x => x.IsValid(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public void CheckLicenseKeyForLowIncomeApplications()//VERIFY PROPERTY GETTER WAS CALLED
        {
            var mockValidator = new Mock<IFrequentFlyerNumberValidator>();

            mockValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication { GrossAnnualIncome = 99_000 };

            sut.Evaluate(application);

            mockValidator.VerifyGet(x => x.ServiceInformation.License.LicenseKey, Times.Once);
        }

        [Fact]
        public void SetDetailedLookupForOlderApplications()//VEFIFY A PRPERTY SETTER WAS CALLED(Ex:validationMode)
        {
            var mockValidator = new Mock<IFrequentFlyerNumberValidator>();

            mockValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication { Age = 30 };

            sut.Evaluate(application);

            //mockValidator.VerifySet(x => x.ValidationMode = ValidationMode.Detailed);//check validation mode set to Detailed
            mockValidator.VerifySet(x => x.ValidationMode = It.IsAny<ValidationMode>(), Times.Once);//check validation mode is set irrespective of what it is set to

            //mockValidator.Verify(x => x.IsValid(null), Times.Once);
            //mockValidator.VerifyNoOtherCalls();//CHECK NO OTHER CALLS MADE APART FROM ABOVE 2.don't check here because there r other calls we r making
        }

        [Fact]
        public void ReferWhenFrequentFlyerValidationError()//THROWING EXCEPTIONS FROM MOCK OBJECTS
        {
            Mock<IFrequentFlyerNumberValidator> mockValidator= new Mock<IFrequentFlyerNumberValidator>();

            mockValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");
            //mockValidator.Setup(x => x.IsValid(It.IsAny<string>())).Returns(true);//this fails since actual is Autodeclined

            //mockValidator.Setup(x => x.IsValid(It.IsAny<string>())).Throws<Exception>();//throws exception when IsValid() called
            //OR
            mockValidator.Setup(x => x.IsValid(It.IsAny<string>()))
                         .Throws(new Exception("Custom message"));

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication { Age = 42 };

            CreditCardApplicationDecision decision = sut.Evaluate(application);

            Assert.Equal(CreditCardApplicationDecision.ReferredToHuman, decision);
        }

        [Fact]
        public void IncrementLookupCount()//RAISING EVENTS FROM MOCK OBJECTS
        {
            Mock<IFrequentFlyerNumberValidator> mockValidator = new Mock<IFrequentFlyerNumberValidator>();
            mockValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");
            mockValidator.Setup(x => x.IsValid(It.IsAny<string>()))
                         .Returns(true)
                         .Raises(x => x.ValidatorLookupPerformed += null, EventArgs.Empty);//OR can raise manually like below

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication { FrequentFlyerNumber = "x", Age = 25 };

            sut.Evaluate(application);

            //OR
            //mockValidator.Raise(x => x.ValidatorLookupPerformed += null, EventArgs.Empty);//it calls the ValidatorLookupPerformed event to be raised on the mock object

            Assert.Equal(1, sut.ValidatorLookupCount);//Check ValidatorLookupCount is incremented and ==1
        }

        [Fact]
        public void ReferInvalidFrequentFlyerApplications_ReturnValuesSequence()//RETURNING DIFFERENT VALUES FOR SEQUENTIAL CALLS.ie,we want mock objects to return different values on multiple calles
        {
            Mock<IFrequentFlyerNumberValidator> mockValidator = new Mock<IFrequentFlyerNumberValidator>();
            mockValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");

            mockValidator.SetupSequence(x => x.IsValid(It.IsAny<string>()))
                         .Returns(false)//return false on 1st call
                         .Returns(true);//return true on 2nd call

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication { Age = 25 };

            CreditCardApplicationDecision firstDecision = sut.Evaluate(application);
            Assert.Equal(CreditCardApplicationDecision.ReferredToHuman, firstDecision);

            CreditCardApplicationDecision secondDecision = sut.Evaluate(application);
            Assert.Equal(CreditCardApplicationDecision.AutoDeclined, secondDecision);
        }

        [Fact]
        public void ReferInvalidFrequentFlyerApplications_MultipleCallsSequence()//CHECK A MOCK METHOD WAS CALLED MULTIPLE TIMES WITH DIFFERENT VAKUES
        {
            Mock<IFrequentFlyerNumberValidator> mockValidator = new Mock<IFrequentFlyerNumberValidator>();
            mockValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");

            var frequentFlyerNumbersPassed = new List<string>();
            mockValidator.Setup(x => x.IsValid(Capture.In(frequentFlyerNumbersPassed)));//capture input argument passed to mock IsValid() method

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application1 = new CreditCardApplication { Age = 25, FrequentFlyerNumber = "aa" };
            var application2 = new CreditCardApplication { Age = 25, FrequentFlyerNumber = "bb" };
            var application3 = new CreditCardApplication { Age = 25, FrequentFlyerNumber = "cc" };

            sut.Evaluate(application1);
            sut.Evaluate(application2);
            sut.Evaluate(application3);

            // Assert that IsValid was called three times with "aa", "bb", and "cc"
            Assert.Equal(new List<string> { "aa", "bb", "cc" }, frequentFlyerNumbersPassed);
        }

        [Fact]
        public void ReferFraudRisk()//MOCKING MEMBERS OF CONCRETE TYPES WITH PARTIAL MOCKS,MOCKING VIRTUAL PROTECTED MEMBERS
                                    //to make use of partial mocks, the thing that we want to injects mock version for has to be virtual.so IsFraudRisk() has to be virtual
        {
            Mock<IFrequentFlyerNumberValidator> mockValidator = new Mock<IFrequentFlyerNumberValidator>();

            Mock<FraudLookup> mockFraudLookup = new Mock<FraudLookup>();
            mockFraudLookup.Setup(x => x.IsFraudRisk(It.IsAny<CreditCardApplication>()))//PARTIAL MOCKING VIRTUAL MEMBER
                           .Returns(true);
            //mockFraudLookup.Protected()//MOCKING PROTECTED MEMBER.above syntax won't work for protected member
            //               .Setup<bool>("CheckApplication", ItExpr.IsAny<CreditCardApplication>())
            //               .Returns(true);

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object,mockFraudLookup.Object);

            var application = new CreditCardApplication();

            CreditCardApplicationDecision decision = sut.Evaluate(application);

            Assert.Equal(CreditCardApplicationDecision.ReferredToHumanFraudRisk, decision);
        }

        [Fact]
        public void LinqToMocks()//IMPROVING MOCK SETUP READABILITY WITH LINQ TO MOCKS
        {
            //--------without LINQ--------------
            //Mock<IFrequentFlyerNumberValidator> mockValidator = new Mock<IFrequentFlyerNumberValidator>();
            //mockValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");
            //mockValidator.Setup(x => x.IsValid(It.IsAny<string>())).Returns(true);

            //var sut = new CreditCardApplicationEvaluator(mockValidator.Object);
            //--------------------------------------------------------------


            IFrequentFlyerNumberValidator mockValidator = Mock.Of<IFrequentFlyerNumberValidator>
                (
                    validitor =>
                    validitor.ServiceInformation.License.LicenseKey == "OK" &&
                    validitor.IsValid(It.IsAny<string>()) == true
                );

            var sut = new CreditCardApplicationEvaluator(mockValidator);


            var application = new CreditCardApplication { Age = 25 };

            CreditCardApplicationDecision decision = sut.Evaluate(application);
            Assert.Equal(CreditCardApplicationDecision.AutoDeclined, decision);
        }
    }
}
    