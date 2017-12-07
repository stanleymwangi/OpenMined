using System;
using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using OpenMined.Network.Controllers;
using OpenMined.Network.Servers;

using OpenMined.Syft.Tensor;
using UnityEditor.VersionControl;

namespace OpenMined.Tests
{

public class FloatTensorGpuTest
{

public SyftController ctrl;
public ComputeShader shader;

public void AssertEqualTensorsData(FloatTensor t1, FloatTensor t2) {

	float[] data1 = new float[t1.Size]; t1.DataBuffer.GetData(data1);
	float[] data2 = new float[t2.Size]; t2.DataBuffer.GetData(data2);

	Assert.AreEqual(t1.DataBuffer.count, t2.DataBuffer.count);
	Assert.AreEqual(t1.DataBuffer.stride, t2.DataBuffer.stride);
	Assert.AreNotEqual(t1.DataBuffer.GetNativeBufferPtr(), t2.DataBuffer.GetNativeBufferPtr());
	Assert.AreEqual(data1, data2);
}

[TestFixtureSetUp]
public void Init()
{
	//Init runs once before running test cases.
	ctrl = new SyftController(null);
	shader = Camera.main.GetComponents<SyftServer>()[0].Shader;
}

[TestFixtureTearDown]
public void CleanUp()
{
	//CleanUp runs once after all test cases are finished.
}

[SetUp]
public void SetUp()
{
	//SetUp runs before all test cases
}

[TearDown]
public void TearDown()
{
	//SetUp runs after all test cases
}

[Test]
public void Copy()
{
	float[] array = { 1, 2, 3, 4, 5 };
	int[] shape = { 5 };

	var tensor = new FloatTensor(_ctrl: ctrl, _data: array, _shape: shape);
	tensor.Gpu(shader);
	var copy = tensor.Copy();
	copy.Gpu(shader);

	Assert.AreEqual(copy.Shape,tensor.Shape);
	Assert.AreNotEqual(copy.Id, tensor.Id);
	AssertEqualTensorsData(tensor, copy);
}


[Test]
public void Cos()
{
	float[] data1 = { 0.4f, 0.5f, 0.3f, -0.1f };
	int[] shape1 = { 4 };
	var tensor = new FloatTensor(_ctrl: ctrl, _data: data1, _shape: shape1);
	tensor.Gpu(shader);

	float[] data2 = { 0.92106099f,  0.87758256f,  0.95533649f,  0.99500417f };
	int[] shape2 = { 4 };
	var expectedCosTensor = new FloatTensor(_ctrl: ctrl, _data: data2, _shape: shape2);
	expectedCosTensor.Gpu(shader);
	var actualCosTensor = tensor.Cos();
	actualCosTensor.Gpu(shader);

	AssertEqualTensorsData(expectedCosTensor, actualCosTensor);
}

[Test]
public void Cos_()
{
	float[] data1 = { 0.4f, 0.5f, 0.3f, -0.1f };
	int[] shape1 = { 4 };
	var tensor = new FloatTensor(_ctrl: ctrl, _data: data1, _shape: shape1);
	tensor.Gpu(shader);
	tensor.Cos (inline: true);

	float[] data2 = {  0.92106099f,  0.87758256f,  0.95533649f,  0.99500417f };
	int[] shape2 = { 4 };
	var expectedCosTensor = new FloatTensor(_ctrl: ctrl, _data: data2, _shape: shape2);
	expectedCosTensor.Gpu(shader);

	AssertEqualTensorsData(tensor, expectedCosTensor);
}

[Test]
public void AddScalar()
{
	float[] data1 = { -1, 0, 0.1f, 1, float.MaxValue, float.MinValue };
	int[] shape1 = {3, 2};
	var tensor1 = new FloatTensor(_ctrl: ctrl, _data: data1, _shape: shape1);
	tensor1.Gpu(shader);
	float[] data2 = { -101, -100, -99.9f, -99, float.MaxValue-100, float.MinValue-100 };
	int[] shape2 = {3, 2};
	var expectedTensor = new FloatTensor(_ctrl: ctrl, _data: data2, _shape: shape2);
	expectedTensor.Gpu(shader);
	float scalar = -100;

	var tensor2 = tensor1.Add (scalar);

	AssertEqualTensorsData(expectedTensor, tensor2);
}

[Test]
public void AddScalar_()
{
	float[] data1 = { -1, 0, 0.1f, 1, float.MaxValue, float.MinValue };
	int[] shape1 = {3, 2};
	var tensor1 = new FloatTensor(_ctrl: ctrl, _data: data1, _shape: shape1);
	tensor1.Gpu(shader);

	float[] data2 = { -101, -100, -99.9f, -99, float.MaxValue-100, float.MinValue-100 };
	int[] shape2 = {3, 2};
	var expectedTensor = new FloatTensor(_ctrl: ctrl, _data: data2, _shape: shape2);
	expectedTensor.Gpu(shader);

	float scalar = -100;

	tensor1.Add (scalar, inline: true);

	AssertEqualTensorsData(expectedTensor, tensor1);
}

[Test]
public void Add()
{
	float[] data1 = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10};
	int[] shape1 = {2, 5};
	var tensor1 = new FloatTensor(_ctrl: ctrl, _data: data1, _shape: shape1);
	tensor1.Gpu(shader);

	float[] data2 = { 3, 2, 6, 9, 10, 1, 4, 8, 5, 7};
	int[] shape2 = {2, 5};
	var tensor2 = new FloatTensor(_ctrl: ctrl, _data: data2, _shape: shape2);
	tensor2.Gpu(shader);

	float[] data3 = { 4, 4, 9, 13, 15, 7, 11, 16, 14, 17 };
	int[] shape3 = {2, 5};
	var expectedTensor = new FloatTensor(_ctrl: ctrl, _data: data3, _shape: shape3);
	expectedTensor.Gpu(shader);

	var tensorSum = tensor1.Add (tensor2);

	AssertEqualTensorsData(expectedTensor, tensorSum);
}

[Test]
public void AddUnequalSizes()
{
	float[] data1 = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
	int[] shape1 = { 2, 5 };
	var tensor1 = new FloatTensor(_ctrl: ctrl, _data: data1, _shape: shape1);
	tensor1.Gpu(shader);

	float[] data2 = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
	int[] shape2 = { 2, 6 };
	var tensor2 = new FloatTensor(_ctrl: ctrl, _data: data2, _shape: shape2);
	tensor2.Gpu(shader);

	Assert.That(() => tensor1.Add(tensor2),
	            Throws.TypeOf<InvalidOperationException>());
}

[Test]
public void AddUnequalDimensions()
{
	float[] data1 = { 1, 2, 3, 4 };
	int[] shape1 = { 4 };
	var tensor1 = new FloatTensor(_ctrl: ctrl, _data: data1, _shape: shape1);

	float[] data2 = { 1, 2, 3, 4 };
	int[] shape2 = { 2, 2 };
	var tensor2 = new FloatTensor(_ctrl: ctrl, _data: data2, _shape: shape2);

	Assert.That(() => tensor1.Add(tensor2),
	            Throws.TypeOf<InvalidOperationException>());
}

[Test]
public void AddUnequalShapes()
{
	float[] data1 = { 1, 2, 3, 4, 5, 6 };
	int[] shape1 = { 2, 3 };
	var tensor1 = new FloatTensor(_ctrl: ctrl, _data: data1, _shape: shape1);
	tensor1.Gpu(shader);

	float[] data2 = { 1, 2, 3, 4, 5, 6 };
	int[] shape2 = { 3, 2 };
	var tensor2 = new FloatTensor(_ctrl: ctrl, _data: data2, _shape: shape2);
	tensor1.Gpu(shader);

	Assert.That(() => tensor1.Add(tensor2),
	            Throws.TypeOf<InvalidOperationException>());
}

[Test]
public void Add_()
{
	float[] data1 = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10};
	int[] shape1 = {2, 5};
	var tensor1 = new FloatTensor(_ctrl: ctrl, _data: data1, _shape: shape1);
	tensor1.Gpu(shader);

	float[] data2 = { 3, 2, 6, 9, 10, 1, 4, 8, 5, 7};
	int[] shape2 = {2, 5};
	var tensor2 = new FloatTensor(_ctrl: ctrl, _data: data2, _shape: shape2);
	tensor2.Gpu(shader);

	float[] data3 = { 4, 4, 9, 13, 15, 7, 11, 16, 14, 17 };
	int[] shape3 = {2, 5};
	var expectedTensor = new FloatTensor(_ctrl: ctrl, _data: data3, _shape: shape3);
	expectedTensor.Gpu(shader);

	tensor1.Add (tensor2, inline: true);

	AssertEqualTensorsData(expectedTensor, tensor1);
}

[Test]
public void AddUnequalSizes_()
{
	float[] data1 = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
	int[] shape1 = { 2, 5 };
	var tensor1 = new FloatTensor(_ctrl: ctrl, _data: data1, _shape: shape1);
	tensor1.Gpu(shader);

	float[] data2 = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
	int[] shape2 = { 2, 6 };
	var tensor2 = new FloatTensor(_ctrl: ctrl, _data: data2, _shape: shape2);
	tensor2.Gpu(shader);

	Assert.That(() => tensor1.Add(tensor2, inline: true),
	            Throws.TypeOf<InvalidOperationException>());
}

[Test]
public void AddUnequalDimensions_()
{
	float[] data1 = { 1, 2, 3, 4 };
	int[] shape1 = { 4 };
	var tensor1 = new FloatTensor(_ctrl: ctrl, _data: data1, _shape: shape1);
	tensor1.Gpu(shader);

	float[] data2 = { 1, 2, 3, 4 };
	int[] shape2 = { 2, 2 };
	var tensor2 = new FloatTensor(_ctrl: ctrl, _data: data2, _shape: shape2);
	tensor2.Gpu(shader);

	Assert.That(() => tensor1.Add(tensor2, inline: true),
	            Throws.TypeOf<InvalidOperationException>());
}

[Test]
public void AddUnequalShapes_()
{
	float[] data1 = { 1, 2, 3, 4, 5, 6 };
	int[] shape1 = { 2, 3 };
	var tensor1 = new FloatTensor(_ctrl: ctrl, _data: data1, _shape: shape1);
	tensor1.Gpu(shader);

	float[] data2 = { 1, 2, 3, 4, 5, 6 };
	int[] shape2 = { 3, 2 };
	var tensor2 = new FloatTensor(_ctrl: ctrl, _data: data2, _shape: shape2);
	tensor2.Gpu(shader);

	Assert.That(() => tensor1.Add(tensor2, inline: true),
	            Throws.TypeOf<InvalidOperationException>());
}



}
}
