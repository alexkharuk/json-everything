﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Json.Pointer;

namespace Json.Schema;

/// <summary>
/// Handles `$dynamicRef`.
/// </summary>
[SchemaKeyword(Name)]
[SchemaDraft(Draft.Draft202012)]
[SchemaDraft(Draft.DraftNext)]
[Vocabulary(Vocabularies.Core202012Id)]
[Vocabulary(Vocabularies.CoreNextId)]
[JsonConverter(typeof(DynamicRefKeywordJsonConverter))]
public class DynamicRefKeyword : IJsonSchemaKeyword, IEquatable<DynamicRefKeyword>
{
	/// <summary>
	/// The JSON name of the keyword.
	/// </summary>
	public const string Name = "$dynamicRef";

	/// <summary>
	/// The URI reference.
	/// </summary>
	public Uri Reference { get; }

	/// <summary>
	/// Creates a new <see cref="DynamicRefKeyword"/>.
	/// </summary>
	/// <param name="value"></param>
	public DynamicRefKeyword(Uri value)
	{
		Reference = value ?? throw new ArgumentNullException(nameof(value));
	}

	/// <summary>
	/// Performs evaluation for the keyword.
	/// </summary>
	/// <param name="context">Contextual details for the evaluation process.</param>
	public void Evaluate(EvaluationContext context)
	{
		context.EnterKeyword(Name);

		var newUri = new Uri(context.Scope.LocalScope, Reference);
		var newBaseUri = new Uri(newUri.GetLeftPart(UriPartial.Query));
		var anchorName = Reference.OriginalString.Split('#').Last();

		JsonSchema? targetSchema = null;
		var targetBase = context.Options.SchemaRegistry.Get(newBaseUri) ??
		                 throw new JsonSchemaException($"Cannot resolve base schema from `{newUri}`");

		foreach (var uri in context.Scope.Reverse())
		{
			var scopeRoot = context.Options.SchemaRegistry.Get(uri);
			if (scopeRoot == null)
				throw new Exception("This shouldn't happen");

			if (!scopeRoot.Anchors.TryGetValue(anchorName, out var anchor) || !anchor.IsDynamic) continue;

			if (context.Options.EvaluatingAs == Draft.Draft202012 &&
			    (!targetBase.Anchors.TryGetValue(anchorName, out var targetAnchor) || !targetAnchor.IsDynamic)) break;

			targetSchema = anchor.Schema;
			break;
		}

		if (targetSchema == null)
		{
			if (JsonPointer.TryParse(newUri.Fragment, out var pointerFragment, JsonPointerKind.UriEncoded))
				targetSchema = targetBase.FindSubschema(pointerFragment!, context.Options);
			else
			{
				anchorName = newUri.Fragment.Substring(1);
				if (!AnchorKeyword.AnchorPattern.IsMatch(anchorName))
					throw new JsonSchemaException($"Unrecognized fragment type `{newUri}`");
			
				if (targetBase.Anchors.TryGetValue(anchorName, out var anchorDefinition))
					targetSchema = anchorDefinition.Schema;
			}

			if (targetSchema == null)
				throw new JsonSchemaException($"Cannot resolve schema `{newUri}`");
		}

		context.Push(context.EvaluationPath.Combine(Name), targetSchema);
		context.Evaluate();
		var result = context.LocalResult.IsValid;
		context.Pop();
		if (!result)
			context.LocalResult.Fail();

		context.ExitKeyword(Name, context.LocalResult.IsValid);
	}

	/// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
	/// <param name="other">An object to compare with this object.</param>
	/// <returns>true if the current object is equal to the <paramref name="other">other</paramref> parameter; otherwise, false.</returns>
	public bool Equals(DynamicRefKeyword? other)
	{
		if (ReferenceEquals(null, other)) return false;
		if (ReferenceEquals(this, other)) return true;
		return Equals(Reference, other.Reference);
	}

	/// <summary>Determines whether the specified object is equal to the current object.</summary>
	/// <param name="obj">The object to compare with the current object.</param>
	/// <returns>true if the specified object  is equal to the current object; otherwise, false.</returns>
	public override bool Equals(object obj)
	{
		return Equals(obj as DynamicRefKeyword);
	}

	/// <summary>Serves as the default hash function.</summary>
	/// <returns>A hash code for the current object.</returns>
	public override int GetHashCode()
	{
		return Reference.GetHashCode();
	}
}

internal class DynamicRefKeywordJsonConverter : JsonConverter<DynamicRefKeyword>
{
	public override DynamicRefKeyword Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		var uri = reader.GetString()!;
		return new DynamicRefKeyword(new Uri(uri, UriKind.RelativeOrAbsolute));


	}
	public override void Write(Utf8JsonWriter writer, DynamicRefKeyword value, JsonSerializerOptions options)
	{
		writer.WritePropertyName(DynamicRefKeyword.Name);
		JsonSerializer.Serialize(writer, value.Reference, options);
	}
}