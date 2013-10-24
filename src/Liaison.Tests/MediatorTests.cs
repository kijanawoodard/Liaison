using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Liaison.Tests
{
    public class MediatorTests
	{
		[Test]
		public void Throws_If_Message_Not_Subscribed()
		{
			var mediator = new Mediator();
			Assert.Throws<ApplicationException>(
				() => mediator.Send<ChangeUserName, string>(new ChangeUserName()));
		}

		[Test]
		public void CanGetResult()
		{
			var mediator = new Mediator();
			mediator.Subscribe<ChangeUserName, string>(message => new ReturnsName().Handle(message));

			var command = new ChangeUserName { Name = "Foo Bar" };
			var result = mediator.Send<ChangeUserName, string>(command);

			Assert.AreEqual("Foo Bar", result);
		}

		[Test]
		public void CanGetResultWithASecondVoidHandler()
		{
			var mediator = new Mediator();

			mediator.Subscribe<ChangeUserName, string>(message =>
			{
				var r = new ReturnsName().Handle(message);
				new ConsoleLogger().Handle(message);
				return r;
			});

			var command = new ChangeUserName { Name = "Foo Bar" };
			var result = mediator.Send<ChangeUserName, string>(command);
			Assert.AreEqual("Foo Bar", result);
		}

		[Test]
		public void CanSendWithoutResult()
		{
			var mediator = new Mediator();

			var counter = new Counter();
			mediator.Subscribe<ChangeUserName>(counter.Handle);

			var command = new ChangeUserName { Name = "Foo Bar" };
			mediator.Send(command);
			Assert.AreEqual(1, counter.Count);
		}

		[Test]
		public void CanSubscribeAIntResultWithoutAFunc()
		{
			var mediator = new Mediator();

			mediator.Subscribe<ChangeUserName, int>(message => new Returns42().Handle(message));

			var command = new ChangeUserName { Name = "Foo Bar" };
			var result = mediator.Send<ChangeUserName, int>(command);
			Assert.AreEqual(42, result);
		}

		[Test]
		public void SecondSubscription_Throws()
		{
			var mediator = new Mediator();
			mediator.Subscribe<ChangeUserName>(message => new NamePersistor().Handle(message));
			Assert.Throws<ArgumentException>(() => mediator.Subscribe<ChangeUserName>(message => new Returns42().Handle(message)));
		}

		[Test]
		public void CanUseMediatorWithinHandler()
		{
			var mediator = new Mediator();
			mediator.Subscribe<ProcessAccount>(message => new AccountExpiditer(mediator).Handle(message));
			mediator.Subscribe<GetAccount, Account>(message => new AccountRepository().Handle(message));
		
			mediator.Send(new ProcessAccount());
		}

		public class ChangeUserName
		{
			public string Name { get; set; }
		}

		public class NamePersistor 
		{
			public void Handle(ChangeUserName message)
			{
				//do persistence
			}
		}

		public class ReturnsName
		{
			public string Handle(ChangeUserName message)
			{
				return message.Name;
			}
		}

		public class ConsoleLogger
		{
			public void Handle(ChangeUserName message)
			{
				Console.WriteLine(message.Name);
			}
		}

		public class Counter
		{
			public int Count { get; set; }

			public void Handle(ChangeUserName message)
			{
				Count++;
			}
		}

		public class Returns42 
		{
			public int Handle(ChangeUserName message)
			{
				return 42;
			}
		}

		public class GenericHook
		{
			public void Handle(object message)
			{

			}
		}

		public class GetUserName
		{

		}

		public class NameViewModel
		{
			public NameViewModel()
			{
			}

			public NameViewModel(string someParam)
			{
			}

			public string Name { get; set; }
		}

		public class SomeRepository
		{
			public SomeRepository()
			{
			}

			public SomeRepository(string someParam)
			{
			}

			public NameViewModel Handle(GetUserName message, NameViewModel result)
			{
				result.Name = "Some Name";
				return result;
			}
		}

		public class Account
		{
			public void Process()
			{
				Console.WriteLine("account processed");
			}
		}

		public class ProcessAccount
		{
			/*account id*/
		}

		public class GetAccount
		{
			/*account id*/
		}

		public class AccountRepository 
		{
			public Account Handle(GetAccount message)
			{
				return new Account(); //return account 
			}
		}

		public class AccountExpiditer
		{
			private readonly IMediator _mediator;

			public AccountExpiditer(IMediator mediator)
			{
				_mediator = mediator;
			}

			public void Handle(ProcessAccount message)
			{
				var account = _mediator.Send<GetAccount, Account>(new GetAccount());
				account.Process();
			}
		}
	}
}
