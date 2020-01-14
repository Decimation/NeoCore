using System;
using SimpleCore.Model;

namespace NeoCore.Utilities
{
	public sealed class UniqueMember : Enumeration
	{
		// Microsoft.VisualBasic.Core/src/Microsoft/VisualBasic/CompilerServices/Symbols.vb


		/// <summary>
		/// Unknown - closure class name
		/// </summary>
		public static readonly UniqueMember ClosureClass = new UniqueMember(1, nameof(ClosureClass))
		{
			Value         = "<>c__DisplayClass",
			ComponentType = IdComponent.Stub,
			NameMangling  = NameMangling.None
		};

		/// <summary>
		/// Unknown - array size class name
		/// </summary>
		public static readonly UniqueMember ArraySizeClass = new UniqueMember(2, nameof(ArraySizeClass))
		{
			Value         = "__StaticArrayInitTypeSize=",
			ComponentType = IdComponent.Stub,
			NameMangling  = NameMangling.None
		};

		/// <summary>
		/// The internal metadata type name of a fixed buffer field.
		/// </summary>
		public static readonly UniqueMember FixedBufferType = new UniqueMember(3, nameof(FixedBufferType))
		{
			Value         = "e__FixedBuffer",
			ComponentType = IdComponent.Suffix,
			NameMangling  = NameMangling.Bracket
		};

		/// <summary>
		/// The internal metadata name of a property's backing field.
		/// </summary>
		public static readonly UniqueMember BackingField = new UniqueMember(4, nameof(BackingField))
		{
			Value         = "k__BackingField",
			ComponentType = IdComponent.Suffix,
			NameMangling  = NameMangling.Bracket
		};

		/// <summary>
		/// Property getter method name
		/// </summary>
		public static readonly UniqueMember Get = new UniqueMember(5, nameof(Get))
		{
			Value         = "get_",
			ComponentType = IdComponent.Prefix,
			NameMangling  = NameMangling.None
		};

		/// <summary>
		/// Property setter method name
		/// </summary>
		public static readonly UniqueMember Set = new UniqueMember(6, nameof(Set))
		{
			Value         = "set_",
			ComponentType = IdComponent.Prefix,
			NameMangling  = NameMangling.None
		};

		/// <summary>
		/// Overloaded operator method name
		/// </summary>
		public static readonly UniqueMember Operator = new UniqueMember(7, nameof(Operator))
		{
			Value         = "op_",
			ComponentType = IdComponent.Prefix,
			NameMangling  = NameMangling.None
		};

		public UniqueMember(int id, string name) : base(id, name) { }

		public string Value { get; private set; }

		public IdComponent ComponentType { get; private set; }

		public NameMangling NameMangling { get; private set; }


		public string GetName(string name)
		{
			switch (NameMangling) {
				case NameMangling.None:
					break;
				case NameMangling.Bracket:
					const string NAME_FORMAT_BRACKET = "<{0}>";
					name = String.Format(NAME_FORMAT_BRACKET, name);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			return ComponentType switch
			{
				IdComponent.Suffix => (name + Value),
				IdComponent.Stub => throw new NotImplementedException(),
				IdComponent.Prefix => (Value + name),
				_ => throw new ArgumentOutOfRangeException()
			};
		}

		public override string ToString()
		{
			return string.Format("{0}: {1}", Name, Value);
		}
	}

	[Flags]
	public enum NameMangling
	{
		None    = 0,
		Bracket = 1 << 0,
	}

	public enum IdComponent
	{
		Stub,
		Prefix,
		Suffix,
	}
}