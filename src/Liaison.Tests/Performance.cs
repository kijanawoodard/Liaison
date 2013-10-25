using System;
using System.Diagnostics;
using NUnit.Framework;

namespace Liaison.Tests
{
	[TestFixture]
	public class Performance
	{
		private const int Iterations = 10 * 1000 * 1000;

		[Test]
		public void Examine()
		{
			Console.WriteLine("----------------------------------------");
			Console.WriteLine("{0} | {1:n0} iterations", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm"), Iterations);
			Console.WriteLine("----------------------------------------");
			var sw = Stopwatch.StartNew();
			for (var i = 0; i < Iterations; i++)
			{

			}
			sw.Stop();
			Console.WriteLine("Baseline: {0}s | {1:n}/ms", sw.Elapsed.TotalSeconds, Math.Round(Iterations / sw.Elapsed.TotalMilliseconds, 2));

			sw = Stopwatch.StartNew();
			for (var i = 0; i < Iterations; i++)
			{
				new Counter().Handle(new DoIteration());
			}
			sw.Stop();
			Console.WriteLine("Manual: {0}s | {1:n}/ms", sw.Elapsed.TotalSeconds, Math.Round(Iterations / sw.Elapsed.TotalMilliseconds, 2));

			var mediator = new Mediator();
			mediator.Subscribe<DoIteration>(message => new Counter().Handle(message));

			sw = Stopwatch.StartNew();
			for (var i = 0; i < Iterations; i++)
			{
				mediator.Send(new DoIteration());
			}
			sw.Stop();
			Console.WriteLine("liason: {0}s | {1:n}/ms", sw.Elapsed.TotalSeconds, Math.Round(Iterations / sw.Elapsed.TotalMilliseconds, 2));
		}

		public class DoIteration { }

		public class Counter
		{
			public int Count { get; set; }

			public void Handle(DoIteration message)
			{
				Count++;
			}
		}
	}
}

/*
----------------------------------------
2013-10-24 22:04 | 10,000,000 iterations
----------------------------------------
Baseline: 0.0237366s | 421,290.33/ms
Manual: 0.3426068s | 29,187.98/ms
liason: 3.3208858s | 3,011.24/ms

----------------------------------------
2013-10-24 23:01 | 10,000,000 iterations - Remove dynamic
----------------------------------------
Baseline: 0.0237256s | 421,485.65/ms
Manual: 0.3527966s | 28,344.94/ms
liason: 2.4162287s | 4,138.68/ms

----------------------------------------
2013-10-25 02:10 | 10,000,000 iterations - sorted dictionary with int key
----------------------------------------
Baseline: 0.0256082s | 390,499.92/ms
Manual: 0.3507434s | 28,510.87/ms
liason: 2.0728059s | 4,824.38/ms
 
----------------------------------------
2013-10-25 14:40 | 10,000,000 iterations - No Tuple for subscription, just message type
----------------------------------------
Baseline: 0.0237612s | 420,854.17/ms
Manual: 0.3404081s | 29,376.50/ms
liason: 1.0718208s | 9,329.92/ms
 
 */