﻿using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Nodes;
using Json.Pointer;

namespace Json.Schema;

/// <summary>
/// Provides a general <see cref="IBaseDocument"/> implementation for a <see cref="JsonNode"/>.
/// </summary>
public class JsonNodeBaseDocument : IBaseDocument
{
	private readonly JsonNode _node;
	private readonly Dictionary<JsonPointer, JsonSchema?> _foundSubschemas;

	/// <summary>
	/// Gets the base URI that applies to this schema.  This may be defined by a parent schema.
	/// </summary>
	/// <remarks>
	/// This property is initialized to a generated random value that matches `https://json-everything.net/{random}`
	/// where `random` is 10 hex characters.
	///
	/// It may change after the initial evaluation based on whether the schema contains an `$id` keyword
	/// or is a child of another schema.
	/// </remarks>
	public Uri BaseUri { get; }

	/// <summary>
	/// Creates a new <see cref="JsonNodeBaseDocument"/>.
	/// </summary>
	/// <param name="node">The JsonNode.</param>
	/// <param name="baseUri">The identifying base URI.</param>
	public JsonNodeBaseDocument(JsonNode node, Uri baseUri)
	{
		_node = node;
		_foundSubschemas = new Dictionary<JsonPointer, JsonSchema?>();

		BaseUri = baseUri;
	}

	/// <summary>
	/// Finds a schema within the document.
	/// </summary>
	/// <param name="pointer">A JSON Pointer to the location of the schema within the document.</param>
	/// <param name="options">Evaluation options.  This is needed for internal processing.</param>
	/// <returns>A JSON Schema, if found.</returns>
	public JsonSchema? FindSubschema(JsonPointer pointer, EvaluationOptions options)
	{
		if (_foundSubschemas.TryGetValue(pointer, out var schema)) return schema;

		if (!pointer.TryEvaluate(_node, out var location)) return null;

		schema = location.Deserialize<JsonSchema>();
		_foundSubschemas[pointer] = schema;

		return schema;
	}
}