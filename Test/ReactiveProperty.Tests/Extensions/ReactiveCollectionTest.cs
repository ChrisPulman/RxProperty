using System;
using System.Reactive.Subjects;
using Microsoft.Reactive.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reactive.Bindings;

namespace ReactiveProperty.Tests.Extensions
{
    [TestClass]
    public class ReactiveCollectionTest
    {
        private TestScheduler scheduler;
        private Subject<int> source;
        private ReactiveCollection<int> target;

        [TestMethod]
        public void Add()
        {
            this.target.Add(0);
            this.target.Add(1);
            this.target.Is(0, 1);
        }

        [TestMethod]
        public void AddSource()
        {
            this.source.OnNext(0);
            this.source.OnNext(1);
            this.scheduler.AdvanceTo(1000);
            this.target.Is(0, 1);
        }

        [TestCleanup]
        public void Cleanup()
        {
            this.scheduler = null;
            this.target = null;
            this.source = null;
        }

        [TestMethod]
        public void Create()
        {
            this.target.Count.Is(0);
        }

        [TestMethod]
        public void Disconnect()
        {
            this.source.OnNext(0);
            this.source.OnNext(1);
            this.scheduler.AdvanceTo(1000);
            this.target.Is(0, 1);

            this.target.Dispose();

            this.source.OnNext(0);
            this.source.OnNext(1);
            this.scheduler.AdvanceTo(1000);
            this.target.Is(0, 1);
        }

        [TestInitialize]
        public void Initialize()
        {
            this.source = new Subject<int>();
            this.scheduler = new TestScheduler();
            this.target = this.source.ToReactiveCollection(scheduler);
        }
    }
}
