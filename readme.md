#What is Liaison?

> a binding ... agent

> a close bond or connection

> one that establishes and maintains communication for mutual understanding and cooperation

> <cite>http://www.merriam-webster.com/dictionary/liaison</cite>

_

> ... a means of communication between different ... units ...

> One that maintains communication

> the relationship ... to ensure unity of purpose

> <cite>http://www.thefreedictionary.com/Liason<cite>

##Summary
An in-memory mediator written in c#. Inspired by [nimbus].

Nimbus is bloated and lacks clarity. [Liaison] seeks balance.

The goal is to isolate the _units_ from each other and separate the organization of the units from the units themselves.

##Rules of engagement

How do we keep from going off the reservation and making spaghetti in our Subscriptions?

* Handlers will have 0 or 1 dependencies.
* The 1 dependency will be the mediator _or_ a fully constructed singleton.
* Prefer a derivation of the singleton - `store.OpenSession()`.
* The singleton should generally be from another library - i.e. persistence lib.
* Handlers will have void/Unit return type _or_ the same return type specified in the Subscribe.

##Usage

	//app start
	mediator.Subscribe<PostRequest, PostGetViewModel>(message =>
	{
		var result = new PostGetViewModel();
		result = new FilteredPostVault().Handle(message, result);
		result = new MarkdownContentStorage(root).Handle(message, result);
		return result;
	});
	
	//somewhere else
	var result = _mediator.Send<PostRequest, PostGetViewModel>(message);

[nimbus]: https://github.com/kijanawoodard/nimbus
[liaison]: http://kijanawoodard.com/introducing-liaison