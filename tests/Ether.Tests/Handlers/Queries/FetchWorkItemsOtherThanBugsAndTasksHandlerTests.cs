﻿using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ether.Vsts.Handlers.Queries;
using Ether.Vsts.Interfaces;
using Ether.Vsts.Queries;
using FizzWare.NBuilder;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using VSTS.Net.Interfaces;
using VSTS.Net.Models.WorkItems;

namespace Ether.Tests.Handlers.Queries
{
    [TestFixture]
    public class FetchWorkItemsOtherThanBugsAndTasksHandlerTests : BaseHandlerTest
    {
        private FetchWorkItemsOtherThanBugsAndTasksHandler _handler;
        private Mock<IVstsClientFactory> _clientFactoryMock;
        private Mock<IVstsClient> _clientMock;

        [Test]
        public async Task ShouldFetchWorkItemsAndWorkItemTypes()
        {
            var workitems = Builder<WorkItem>.CreateListOfSize(5)
                .Build();
            var ids = workitems.Select(w => w.Id).ToArray();

            _clientMock.Setup(c => c.GetWorkItemsAsync(ids, null, It.IsAny<string[]>(), default(CancellationToken)))
                .ReturnsAsync(workitems)
                .Verifiable();

            var result = await _handler.Handle(new FetchWorkItemsOtherThanBugsAndTasks());

            result.Should().HaveCountLessOrEqualTo(workitems.Count);
            result.Should().OnlyContain(i => workitems.Any(w => w.Id == i));

            _clientFactoryMock.Verify();
            _clientMock.Verify();
        }

        protected override void Initialize()
        {
            _clientFactoryMock = new Mock<IVstsClientFactory>(MockBehavior.Strict);
            _clientMock = new Mock<IVstsClient>(MockBehavior.Strict);
            _handler = new FetchWorkItemsOtherThanBugsAndTasksHandler(_clientFactoryMock.Object, RepositoryMock.Object);

            _clientFactoryMock.Setup(c => c.GetClient(null))
                .ReturnsAsync(_clientMock.Object)
                .Verifiable();
        }
    }
}