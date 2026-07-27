#load "mqtt_thing_model.cs"

Console.WriteLine("=== mqtt_thing_model Test ===");

try
{
    var t0 = typeof(ThingModelBase);
    Console.WriteLine($"[PASS] ThingModelBase 存在");
    var t1 = typeof(ThingModelSchema);
    Console.WriteLine($"[PASS] ThingModelSchema 存在");
    var t2 = typeof(ThingModelProfile);
    Console.WriteLine($"[PASS] ThingModelProfile 存在");
    var t3 = typeof(ThingModelProperty);
    Console.WriteLine($"[PASS] ThingModelProperty 存在");
    var t4 = typeof(ThingModelEvent);
    Console.WriteLine($"[PASS] ThingModelEvent 存在");
    var t5 = typeof(ThingModelAction);
    Console.WriteLine($"[PASS] ThingModelAction 存在");
    var t6 = typeof(ThingModelFunctionBlock);
    Console.WriteLine($"[PASS] ThingModelFunctionBlock 存在");
    var t7 = typeof(ThingModelData);
    Console.WriteLine($"[PASS] ThingModelData 存在");
    var t8 = typeof(ThingModelDataType);
    Console.WriteLine($"[PASS] ThingModelDataType 存在");
    var t9 = typeof(ThingModelDataSpecs);
    Console.WriteLine($"[PASS] ThingModelDataSpecs 存在");
    var t10 = typeof(ThingModelDataRange);
    Console.WriteLine($"[PASS] ThingModelDataRange 存在");
    var t11 = typeof(MqttThingModel);
    Console.WriteLine($"[PASS] MqttThingModel 存在");
    var t12 = typeof(DeviceMessage);
    Console.WriteLine($"[PASS] DeviceMessage 存在");
    var t13 = typeof(ThingModelResonanceAdapter);
    Console.WriteLine($"[PASS] ThingModelResonanceAdapter 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}