using JobApplicationLibrary.Models;
using NUnit.Framework;
using Moq;
using JobApplicationLibrary.Services;
using FluentAssertions;
using System;

namespace JobApplicationLibrary.UnitTest
{
    public class ApplicationEvaluateUnitTest
    {
        //UnitOfWork_Condition_ExpectedResult
        //UnitOfWork_ExperienceResult_Condition
        //Isimlendirme kurallari

        [Test]
        public void Application_WithUnderAge_TransferredToAutoRejected()
        {
            var evaluator = new ApplicationEvaluator(null);
            var form = new JobApplication()
            {
                Applicant = new Applicant()
                {
                    Age = 17
                }
            };

            var appResult = evaluator.Evaluate(form);
            //Assert.AreEqual(appResult, ApplicationResult.AutoRejected);
            appResult.Should().Be(ApplicationResult.AutoRejected);
        }



        [Test]
        public void Application_WithNoTechStack_TransferredToAutoRejected()
        {
            var mockValidator = new Mock<IIdentityValidator>();
            mockValidator.DefaultValue = DefaultValue.Mock;
            mockValidator.Setup(i => i.CountryDataProvider.CountryData.Country).Returns("TURKEY");
            mockValidator.Setup(i => i.IsValid(It.IsAny<string>())).Returns(true);
            var evaluator = new ApplicationEvaluator(mockValidator.Object);
            var form = new JobApplication()
            {
                Applicant = new Applicant() { Age=19},
                TechStackList=new System.Collections.Generic.List<string>() { ""}
            };

            var appResult = evaluator.Evaluate(form);
            //Assert.AreEqual(appResult, ApplicationResult.AutoRejected);
            appResult.Should().Be(ApplicationResult.AutoRejected);
        }

        [Test]
        public void Application_WithTechStackOver75P_TransferredToAutoAccepted()
        {
            var mockValidator = new Mock<IIdentityValidator>();
            mockValidator.DefaultValue = DefaultValue.Mock;
            mockValidator.Setup(i => i.CountryDataProvider.CountryData.Country).Returns("TURKEY");
            mockValidator.Setup(i => i.IsValid(It.IsAny<string>())).Returns(true);

            var evaluator = new ApplicationEvaluator(mockValidator.Object);
            var form = new JobApplication()
            {
                Applicant = new Applicant() { Age = 19 },
                TechStackList = new System.Collections.Generic.List<string>() { "C#", "RabbitMQ", "Microservice", "Visual Studio" },YearsOfExperience = 16
            };

            var appResult = evaluator.Evaluate(form);
            //Assert.AreEqual(appResult, ApplicationResult.AutoAccepted);
            appResult.Should().Be(ApplicationResult.AutoAccepted);
        }


        [Test]
        public void Application_WithInValidIdentityNumber_TransferredToHR()
        {
            var mockValidator = new Mock<IIdentityValidator>();
            mockValidator.DefaultValue = DefaultValue.Mock;
            mockValidator.Setup(i => i.CountryDataProvider.CountryData.Country).Returns("TURKEY");
            mockValidator.Setup(i => i.IsValid(It.IsAny<string>())).Returns(false);

            var evaluator = new ApplicationEvaluator(mockValidator.Object);
            var form = new JobApplication()
            {
                Applicant = new Applicant() { Age = 19 }
            };

            var appResult = evaluator.Evaluate(form);
            //Assert.AreEqual( ApplicationResult.TransferredToHR,appResult);
            appResult.Should().Be(ApplicationResult.TransferredToHR);
        }


        [Test]
        public void Application_WithOfficeLocation_TransferredToCTO()
        {
            var mockValidator = new Mock<IIdentityValidator>();
            mockValidator.DefaultValue = DefaultValue.Mock;
            mockValidator.Setup(i => i.CountryDataProvider.CountryData.Country).Returns("SPAIN");

            var evaluator = new ApplicationEvaluator(mockValidator.Object);
            var form = new JobApplication()
            {
                Applicant = new Applicant() { Age = 19 },
            };

            var appResult = evaluator.Evaluate(form);
            //Assert.AreEqual(ApplicationResult.TransferredToCTO, appResult);
            appResult.Should().Be(ApplicationResult.TransferredToCTO);
        }

        [Test]
        public void Application_WithOver50_ValidationModeToDetailed()
        {
            var mockValidator = new Mock<IIdentityValidator>();

            mockValidator.SetupAllProperties();

            mockValidator.Setup(i => i.CountryDataProvider.CountryData.Country).Returns("SPAIN");

            var evaluator = new ApplicationEvaluator(mockValidator.Object);
            var form = new JobApplication()
            {
                Applicant = new Applicant() { Age = 51 },
            };

            var appResult = evaluator.Evaluate(form);
            //Assert.AreEqual(ValidationMode.Detailed,mockValidator.Object.ValidationMode);
            mockValidator.Object.ValidationMode.Should().Be(ValidationMode.Detailed);
        }

        [Test]
        public void Application_WithNullApplication_ThrowsArgumentNullException()
        {
            var mockValidator = new Mock<IIdentityValidator>();
            var evaluator = new ApplicationEvaluator(mockValidator.Object);
            var form = new JobApplication();

            Action appResultAction = () => evaluator.Evaluate(form);
            appResultAction.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void Application_WithDefaultValue_IsValidCalled()
        {
            var mockValidator = new Mock<IIdentityValidator>();
            mockValidator.DefaultValue = DefaultValue.Mock;
            mockValidator.Setup(i => i.CountryDataProvider.CountryData.Country).Returns("TURKEY");

            var evaluator = new ApplicationEvaluator(mockValidator.Object);
            var form = new JobApplication()
            {
                Applicant=new Applicant()
                {
                    Age=19,
                    IdentityNumber="123"
                }
            };
            var appResult = evaluator.Evaluate(form);
            mockValidator.Verify(i => i.IsValid(It.IsAny<string>()),"IsValidMethod should be called with 123");
        }

        [Test]
        public void Application_WithYoungAge_IsValidNeverCalled()
        {
            var mockValidator = new Mock<IIdentityValidator>();
            mockValidator.DefaultValue = DefaultValue.Mock;
            mockValidator.Setup(i => i.CountryDataProvider.CountryData.Country).Returns("TURKEY");

            var evaluator = new ApplicationEvaluator(mockValidator.Object);
            var form = new JobApplication()
            {
                Applicant = new Applicant()
                {
                    Age = 15
                }
            };
            var appResult = evaluator.Evaluate(form);
            mockValidator.Verify(i => i.IsValid(It.IsAny<string>()),Times.Never()); //Times.Exactly(2) kaç kere cagrılması gerektigini söylüyor
        }


    }
}
 