﻿using NUnit.Framework;

using static Json.Schema.DataGeneration.Tests.TestHelpers;

namespace Json.Schema.DataGeneration.Tests
{
	public class ArrayGenerationTests
	{
		[Test]
		public void GenerateArrayOfNumbers()
		{
			JsonSchema schema = new JsonSchemaBuilder()
				.Type(SchemaValueType.Array)
				.Items(new JsonSchemaBuilder()
					.Type(SchemaValueType.Integer)
					.Minimum(10)
					.Maximum(20)
					.MultipleOf(2)
				)
				.MinItems(3)
				.MaxItems(10);

			Run(schema);
		}

		[Test]
		public void GenerateArrayOfNumbersNoMax()
		{
			JsonSchema schema = new JsonSchemaBuilder()
				.Type(SchemaValueType.Array)
				.Items(new JsonSchemaBuilder()
					.Type(SchemaValueType.Integer)
					.Minimum(10)
					.Maximum(20)
					.MultipleOf(2)
				)
				.MinItems(3);

			Run(schema);
		}

		[Test]
		public void GenerateArrayOfNumbersWithAllOf()
		{
			JsonSchema schema = new JsonSchemaBuilder()
				.Type(SchemaValueType.Array)
				.AllOf(
					new JsonSchemaBuilder()
						.Items(new JsonSchemaBuilder().MultipleOf(3)),
					new JsonSchemaBuilder()
						.Items(new JsonSchemaBuilder().MultipleOf(2))
				)
				.Items(new JsonSchemaBuilder()
					.Type(SchemaValueType.Integer)
					.Minimum(10)
					.Maximum(50)
				)
				.MinItems(3)
				.MaxItems(10);

			Run(schema);
		}

		[Test]
		public void GenerateArrayOfNumbersWithAnyOf()
		{
			JsonSchema schema = new JsonSchemaBuilder()
				.Type(SchemaValueType.Array)
				.AnyOf(
					new JsonSchemaBuilder()
						.Items(new JsonSchemaBuilder().MultipleOf(3)),
					new JsonSchemaBuilder()
						.Items(new JsonSchemaBuilder().MultipleOf(2))
				)
				.Items(new JsonSchemaBuilder()
					.Type(SchemaValueType.Integer)
					.Minimum(10)
					.Maximum(50)
				)
				.MinItems(3)
				.MaxItems(10);

			Run(schema);
		}

		[Test]
		public void GenerateArrayThatContains100()
		{
			JsonSchema schema = new JsonSchemaBuilder()
				.Type(SchemaValueType.Array)
				.Items(new JsonSchemaBuilder()
					.Type(SchemaValueType.Integer)
					.Minimum(10)
					.Maximum(100)
				)
				.Contains(new JsonSchemaBuilder()
					.Minimum(100)
				)
				.MinItems(3)
				.MaxItems(10);

			Run(schema);
		}

		[Test]
		public void GenerateArrayThatContainsAtLeastTwo100s()
		{
			JsonSchema schema = new JsonSchemaBuilder()
				.Type(SchemaValueType.Array)
				.Items(new JsonSchemaBuilder()
					.Type(SchemaValueType.Integer)
					.Minimum(10)
					.Maximum(100)
				)
				.Contains(new JsonSchemaBuilder()
					.Minimum(100)
				)
				.MinContains(2)
				.MinItems(3)
				.MaxItems(10);

			Run(schema);
		}

		[Test]
		public void GenerateArrayThatContainsAtMostThree100s()
		{
			JsonSchema schema = new JsonSchemaBuilder()
				.Type(SchemaValueType.Array)
				.Items(new JsonSchemaBuilder()
					.Type(SchemaValueType.Integer)
					.Minimum(10)
					.Maximum(100)
				)
				.Contains(new JsonSchemaBuilder()
					.Minimum(100)
				)
				.MaxContains(2)
				.MinItems(3)
				.MaxItems(10);

			Run(schema);
		}

		[Test]
		public void GenerateArrayThatContainsBetweenTwoAndFive100s()
		{
			JsonSchema schema = new JsonSchemaBuilder()
				.Type(SchemaValueType.Array)
				.Items(new JsonSchemaBuilder()
					.Type(SchemaValueType.Integer)
					.Minimum(10)
					.Maximum(100)
				)
				.Contains(new JsonSchemaBuilder()
					.Minimum(100)
				)
				.MinContains(2)
				.MaxContains(5)
				.MinItems(3)
				.MaxItems(20);

			Run(schema);
		}
	}
}
