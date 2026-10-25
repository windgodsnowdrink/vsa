// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using HslCommunication.Core.IMessage;
using HslCommunication.Core.Plugin;
using HslCommunication.Reflection;

namespace HslCommunication.Core.Device;

/// <summary>
/// 插件设备对象，插件设备定义一些方法就可以使用HSL的全部方法，具体方法参考官网文档及skill说明
/// Plugin device object. By defining some methods for the plugin device, all the methods of HSL can be used. For specific methods, please refer to the official website documentation and the skill instructions.
/// </summary>
public class DevicePluginNet : DeviceTcpNet
{
	private Func<string, bool> readBoolSingleFunc = null;

	private Func<string, byte> readByteFunc = null;

	private Func<string, short> readInt16Func = null;

	private Func<string, ushort, short[]> readInt16ArrayFunc = null;

	private Func<string, ushort> readUInt16Func = null;

	private Func<string, ushort, ushort[]> readUInt16ArrayFunc = null;

	private Func<string, int> readInt32Func = null;

	private Func<string, ushort, int[]> readInt32ArrayFunc = null;

	private Func<string, uint> readUInt32Func = null;

	private Func<string, ushort, uint[]> readUInt32ArrayFunc = null;

	private Func<string, long> readInt64Func = null;

	private Func<string, ushort, long[]> readInt64ArrayFunc = null;

	private Func<string, ulong> readUInt64Func = null;

	private Func<string, ushort, ulong[]> readUInt64ArrayFunc = null;

	private Func<string, float> readFloatFunc = null;

	private Func<string, ushort, float[]> readFloatArrayFunc = null;

	private Func<string, double> readDoubleFunc = null;

	private Func<string, ushort, double[]> readDoubleArrayFunc = null;

	private Func<string, ushort, Encoding, string> readStringFunc = null;

	private Func<string, bool, string> writeBoolSingleFunc = null;

	private Func<string, byte, string> writeByteFunc = null;

	private Func<string, short, string> writeInt16Func = null;

	private Func<string, short[], string> writeInt16ArrayFunc = null;

	private Func<string, ushort, string> writeUInt16Func = null;

	private Func<string, ushort[], string> writeUInt16ArrayFunc = null;

	private Func<string, int, string> writeInt32Func = null;

	private Func<string, int[], string> writeInt32ArrayFunc = null;

	private Func<string, uint, string> writeUInt32Func = null;

	private Func<string, uint[], string> writeUInt32ArrayFunc = null;

	private Func<string, long, string> writeInt64Func = null;

	private Func<string, long[], string> writeInt64ArrayFunc = null;

	private Func<string, ulong, string> writeUInt64Func = null;

	private Func<string, ulong[], string> writeUInt64ArrayFunc = null;

	private Func<string, float, string> writeFloatFunc = null;

	private Func<string, float[], string> writeFloatArrayFunc = null;

	private Func<string, double, string> writeDoubleFunc = null;

	private Func<string, double[], string> writeDoubleArrayFunc = null;

	private Func<string, string, Encoding, string> writeStringEncodingFunc = null;

	private Func<string, ushort, bool, List<byte[]>> BuildDeviceReadCommandFunc = null;

	private Func<string, bool[], byte[]> BuildDeviceWriteBoolCommandFunc = null;

	private Func<string, byte[], byte[]> BuildDeviceWriteByteCommandFunc = null;

	private Func<string, byte[], byte[], string> CheckDeviceResponseFunc = null;

	private Func<string, byte[], byte[], byte[]> ParseDeviceReadResultFunc = null;

	private Func<string, byte[], byte[], ushort, bool[]> ParseDeviceReadBoolResultFunc = null;

	private Func<int> InitializationOnConnectFunc = null;

	private Func<int> ProtocolHeadBytesLengthFunc = null;

	private Func<byte[], byte[], int> GetContentLengthByHeadBytesFunc = null;

	private Func<byte[], byte[], bool> CheckReceiveDataCompleteFunc = null;

	private Func<string, ushort, byte[]> ReadBytesFunc = null;

	private Func<string, ushort, bool[]> ReadBoolFunc = null;

	private Func<string, byte[], string> WriteBytesFunc = null;

	private Func<string, bool[], string> WriteBoolFunc = null;

	private Func<string, ushort, Task<byte[]>> ReadBytesAsyncFunc = null;

	private Func<string, ushort, Task<bool[]>> ReadBoolAsyncFunc = null;

	private Func<string, byte[], Task<string>> WriteBytesAsyncFunc = null;

	private Func<string, bool[], Task<string>> WriteBoolAsyncFunc = null;

	private Func<string, Task<bool>> readBoolSingleAsyncFunc = null;

	private Func<string, Task<byte>> readByteAsyncFunc = null;

	private Func<string, Task<short>> readInt16AsyncFunc = null;

	private Func<string, ushort, Task<short[]>> readInt16ArrayAsyncFunc = null;

	private Func<string, Task<ushort>> readUInt16AsyncFunc = null;

	private Func<string, ushort, Task<ushort[]>> readUInt16ArrayAsyncFunc = null;

	private Func<string, Task<int>> readInt32AsyncFunc = null;

	private Func<string, ushort, Task<int[]>> readInt32ArrayAsyncFunc = null;

	private Func<string, Task<uint>> readUInt32AsyncFunc = null;

	private Func<string, ushort, Task<uint[]>> readUInt32ArrayAsyncFunc = null;

	private Func<string, Task<long>> readInt64AsyncFunc = null;

	private Func<string, ushort, Task<long[]>> readInt64ArrayAsyncFunc = null;

	private Func<string, Task<ulong>> readUInt64AsyncFunc = null;

	private Func<string, ushort, Task<ulong[]>> readUInt64ArrayAsyncFunc = null;

	private Func<string, Task<float>> readFloatAsyncFunc = null;

	private Func<string, ushort, Task<float[]>> readFloatArrayAsyncFunc = null;

	private Func<string, Task<double>> readDoubleAsyncFunc = null;

	private Func<string, ushort, Task<double[]>> readDoubleArrayAsyncFunc = null;

	private Func<string, ushort, Encoding, Task<string>> readStringAsyncFunc = null;

	private Func<string, bool, Task<string>> writeBoolSingleAsyncFunc = null;

	private Func<string, byte, Task<string>> writeByteAsyncFunc = null;

	private Func<string, short, Task<string>> writeInt16AsyncFunc = null;

	private Func<string, short[], Task<string>> writeInt16ArrayAsyncFunc = null;

	private Func<string, ushort, Task<string>> writeUInt16AsyncFunc = null;

	private Func<string, ushort[], Task<string>> writeUInt16ArrayAsyncFunc = null;

	private Func<string, int, Task<string>> writeInt32AsyncFunc = null;

	private Func<string, int[], Task<string>> writeInt32ArrayAsyncFunc = null;

	private Func<string, uint, Task<string>> writeUInt32AsyncFunc = null;

	private Func<string, uint[], Task<string>> writeUInt32ArrayAsyncFunc = null;

	private Func<string, long, Task<string>> writeInt64AsyncFunc = null;

	private Func<string, long[], Task<string>> writeInt64ArrayAsyncFunc = null;

	private Func<string, ulong, Task<string>> writeUInt64AsyncFunc = null;

	private Func<string, ulong[], Task<string>> writeUInt64ArrayAsyncFunc = null;

	private Func<string, float, Task<string>> writeFloatAsyncFunc = null;

	private Func<string, float[], Task<string>> writeFloatArrayAsyncFunc = null;

	private Func<string, double, Task<string>> writeDoubleAsyncFunc = null;

	private Func<string, double[], Task<string>> writeDoubleArrayAsyncFunc = null;

	private Func<string, string, Encoding, Task<string>> writeStringEncodingAsyncFunc = null;

	private Func<string> connectServerFunc = null;

	private Func<string> connectClose = null;

	private Func<Task<string>> connectServerAsyncFunc = null;

	private Func<Task<string>> connectCloseAsyncFunc = null;

	private MethodInfo setParametersMethod = null;

	private object pluginObject = null;

	private DevicePluginMessage pluginMessage = null;

	/// <summary>
	/// 指定一个自定义的用户对象来创建插件设备
	/// </summary>
	public DevicePluginNet(object deviceObject)
	{
		base.ByteTransform = new RegularByteTransform();
		pluginMessage = new DevicePluginMessage();
		SetDeviceObject(deviceObject);
	}

	private void SetDeviceObject(object deviceObject)
	{
		pluginObject = deviceObject;
		Type type = deviceObject.GetType();
		BuildDeviceReadCommandFunc = PluginsHelper.ExecuteFuncThree<string, ushort, bool, List<byte[]>>(type, deviceObject, "BuildDeviceReadCommand", new string[3] { "address", "length", "isBit" });
		BuildDeviceWriteBoolCommandFunc = PluginsHelper.ExecuteFuncTwo<string, bool[], byte[]>(type, deviceObject, "BuildDeviceWriteCommand", new string[2] { "address", "value" });
		BuildDeviceWriteByteCommandFunc = PluginsHelper.ExecuteFuncTwo<string, byte[], byte[]>(type, deviceObject, "BuildDeviceWriteCommand", new string[2] { "address", "value" });
		CheckDeviceResponseFunc = PluginsHelper.ExecuteFuncThree<string, byte[], byte[], string>(type, deviceObject, "CheckDeviceResponse", new string[3] { "address", "buildCommand", "content" });
		ParseDeviceReadResultFunc = PluginsHelper.ExecuteFuncThree<string, byte[], byte[], byte[]>(type, deviceObject, "ParseDeviceReadResult", new string[3] { "address", "buildCommand", "content" });
		ParseDeviceReadBoolResultFunc = PluginsHelper.ExecuteFuncFour<string, byte[], byte[], ushort, bool[]>(type, deviceObject, "ParseDeviceReadBoolResult", new string[4] { "address", "buildCommand", "content", "ushort" });
		ProtocolHeadBytesLengthFunc = PluginsHelper.ExecuteFuncZero<int>(type, deviceObject, "ProtocolHeadBytesLength", Array.Empty<string>());
		GetContentLengthByHeadBytesFunc = PluginsHelper.ExecuteFuncTwo<byte[], byte[], int>(type, deviceObject, "GetContentLengthByHeadBytes", new string[2] { "send", "head" });
		CheckReceiveDataCompleteFunc = PluginsHelper.ExecuteFuncTwo<byte[], byte[], bool>(type, deviceObject, "CheckReceiveDataComplete", new string[2] { "send", "content" });
		ReadBytesFunc = PluginsHelper.ExecuteFuncTwo<string, ushort, byte[]>(type, deviceObject, "Read", new string[2] { "address", "length" });
		ReadBoolFunc = PluginsHelper.ExecuteFuncTwo<string, ushort, bool[]>(type, deviceObject, "ReadBool", new string[2] { "address", "length" });
		WriteBytesFunc = PluginsHelper.ExecuteFuncTwo<string, byte[], string>(type, deviceObject, "Write", new string[2] { "address", "value" });
		WriteBoolFunc = PluginsHelper.ExecuteFuncTwo<string, bool[], string>(type, deviceObject, "Write", new string[2] { "address", "value" });
		ReadBytesAsyncFunc = PluginsHelper.ExecuteFuncTwo<string, ushort, Task<byte[]>>(type, deviceObject, "ReadAsync", new string[2] { "address", "length" });
		ReadBoolAsyncFunc = PluginsHelper.ExecuteFuncTwo<string, ushort, Task<bool[]>>(type, deviceObject, "ReadBoolAsync", new string[2] { "address", "length" });
		WriteBytesAsyncFunc = PluginsHelper.ExecuteFuncTwo<string, byte[], Task<string>>(type, deviceObject, "WriteAsync", new string[2] { "address", "value" });
		WriteBoolAsyncFunc = PluginsHelper.ExecuteFuncTwo<string, bool[], Task<string>>(type, deviceObject, "WriteAsync", new string[2] { "address", "value" });
		if (ProtocolHeadBytesLengthFunc != null)
		{
			pluginMessage.ProtocolHeadBytesLengthFunc = ProtocolHeadBytesLengthFunc;
		}
		if (GetContentLengthByHeadBytesFunc != null)
		{
			pluginMessage.GetContentLengthByHeadBytesFunc = GetContentLengthByHeadBytesFunc;
		}
		if (CheckReceiveDataCompleteFunc != null)
		{
			pluginMessage.CheckReceiveDataCompleteFunc = CheckReceiveDataCompleteFunc;
		}
		InitializationOnConnectFunc = PluginsHelper.ExecuteFuncZero<int>(type, deviceObject, "InitializationOnConnect", Array.Empty<string>());
		PropertyInfo property = type.GetProperty("ReadFromCoreServerFunc");
		if (property != null && property.PropertyType == typeof(Func<byte[], bool, bool, byte[]>) && property.CanWrite)
		{
			property.SetValue(deviceObject, new Func<byte[], bool, bool, byte[]>(ReadFromCoreServerHelper));
		}
		PropertyInfo property2 = type.GetProperty("ReadFromCoreServer");
		if (property2 != null && property2.PropertyType == typeof(Func<byte[], byte[]>) && property2.CanWrite)
		{
			property2.SetValue(deviceObject, new Func<byte[], byte[]>(ReadFromCoreServerHelper2));
		}
		PropertyInfo property3 = type.GetProperty("ReadFromCoreServerAsyncFunc");
		if (property3 != null && property3.PropertyType == typeof(Func<byte[], bool, bool, Task<byte[]>>) && property3.CanWrite)
		{
			property3.SetValue(deviceObject, new Func<byte[], bool, bool, Task<byte[]>>(ReadFromCoreServerHelperAsync), null);
		}
		PropertyInfo property4 = type.GetProperty("ReadFromCoreServerAsync");
		if (property4 != null && property4.PropertyType == typeof(Func<byte[], Task<byte[]>>) && property4.CanWrite)
		{
			property4.SetValue(deviceObject, new Func<byte[], Task<byte[]>>(ReadFromCoreServerHelper2Async));
		}
		PropertyInfo property5 = type.GetProperty("DataFormat", new Type[0]);
		if (property5 != null && property5.PropertyType == typeof(string))
		{
			string text = property5.GetValue(deviceObject, null).ToString();
			if (!string.IsNullOrEmpty(text))
			{
				if (text.Equals("ABCD", StringComparison.OrdinalIgnoreCase))
				{
					base.ByteTransform.DataFormat = DataFormat.ABCD;
				}
				else if (text.Equals("BADC", StringComparison.OrdinalIgnoreCase))
				{
					base.ByteTransform.DataFormat = DataFormat.BADC;
				}
				else if (text.Equals("CDAB", StringComparison.OrdinalIgnoreCase))
				{
					base.ByteTransform.DataFormat = DataFormat.CDAB;
				}
				else if (text.Equals("DCBA", StringComparison.OrdinalIgnoreCase))
				{
					base.ByteTransform.DataFormat = DataFormat.DCBA;
				}
			}
		}
		PropertyInfo property6 = type.GetProperty("WordLength", new Type[0]);
		if (property6 != null && property6.PropertyType == typeof(ushort))
		{
			base.WordLength = (ushort)property6.GetValue(deviceObject, null);
		}
		setParametersMethod = pluginObject.GetType().GetMethod("SetParameters");
		connectServerFunc = PluginsHelper.ExecuteFuncZero<string>(type, deviceObject, "ConnectServer", Array.Empty<string>());
		connectClose = PluginsHelper.ExecuteFuncZero<string>(type, deviceObject, "ConnectClose", Array.Empty<string>());
		connectServerAsyncFunc = PluginsHelper.ExecuteFuncZero<Task<string>>(type, deviceObject, "ConnectServerAsync", Array.Empty<string>());
		connectCloseAsyncFunc = PluginsHelper.ExecuteFuncZero<Task<string>>(type, deviceObject, "ConnectCloseAsync", Array.Empty<string>());
		readBoolSingleFunc = PluginsHelper.ExecuteFuncOne<string, bool>(type, deviceObject, "ReadBool", new string[1] { "address" });
		readByteFunc = PluginsHelper.ExecuteFuncOne<string, byte>(type, deviceObject, "ReadByte", new string[1] { "address" });
		readInt16Func = PluginsHelper.ExecuteFuncOne<string, short>(type, deviceObject, "ReadInt16", new string[1] { "address" });
		readInt16ArrayFunc = PluginsHelper.ExecuteFuncTwo<string, ushort, short[]>(type, deviceObject, "ReadInt16", new string[2] { "address", "length" });
		readUInt16Func = PluginsHelper.ExecuteFuncOne<string, ushort>(type, deviceObject, "ReadUInt16", new string[1] { "address" });
		readUInt16ArrayFunc = PluginsHelper.ExecuteFuncTwo<string, ushort, ushort[]>(type, deviceObject, "ReadUInt16", new string[2] { "address", "length" });
		readInt32Func = PluginsHelper.ExecuteFuncOne<string, int>(type, deviceObject, "ReadInt32", new string[1] { "address" });
		readInt32ArrayFunc = PluginsHelper.ExecuteFuncTwo<string, ushort, int[]>(type, deviceObject, "ReadInt32", new string[2] { "address", "length" });
		readUInt32Func = PluginsHelper.ExecuteFuncOne<string, uint>(type, deviceObject, "ReadUInt32", new string[1] { "address" });
		readUInt32ArrayFunc = PluginsHelper.ExecuteFuncTwo<string, ushort, uint[]>(type, deviceObject, "ReadUInt32", new string[2] { "address", "length" });
		readInt64Func = PluginsHelper.ExecuteFuncOne<string, long>(type, deviceObject, "ReadInt64", new string[1] { "address" });
		readInt64ArrayFunc = PluginsHelper.ExecuteFuncTwo<string, ushort, long[]>(type, deviceObject, "ReadInt64", new string[2] { "address", "length" });
		readUInt64Func = PluginsHelper.ExecuteFuncOne<string, ulong>(type, deviceObject, "ReadUInt64", new string[1] { "address" });
		readUInt64ArrayFunc = PluginsHelper.ExecuteFuncTwo<string, ushort, ulong[]>(type, deviceObject, "ReadUInt64", new string[2] { "address", "length" });
		readFloatFunc = PluginsHelper.ExecuteFuncOne<string, float>(type, deviceObject, "ReadFloat", new string[1] { "address" });
		readFloatArrayFunc = PluginsHelper.ExecuteFuncTwo<string, ushort, float[]>(type, deviceObject, "ReadFloat", new string[2] { "address", "length" });
		readDoubleFunc = PluginsHelper.ExecuteFuncOne<string, double>(type, deviceObject, "ReadDouble", new string[1] { "address" });
		readDoubleArrayFunc = PluginsHelper.ExecuteFuncTwo<string, ushort, double[]>(type, deviceObject, "ReadDouble", new string[2] { "address", "length" });
		readStringFunc = PluginsHelper.ExecuteFuncThree<string, ushort, Encoding, string>(type, deviceObject, "ReadString", new string[3] { "address", "length", "encoding" });
		writeBoolSingleFunc = PluginsHelper.ExecuteFuncTwo<string, bool, string>(type, deviceObject, "Write", new string[2] { "address", "value" });
		writeByteFunc = PluginsHelper.ExecuteFuncTwo<string, byte, string>(type, deviceObject, "Write", new string[2] { "address", "value" });
		writeInt16Func = PluginsHelper.ExecuteFuncTwo<string, short, string>(type, deviceObject, "Write", new string[2] { "address", "value" });
		writeInt16ArrayFunc = PluginsHelper.ExecuteFuncTwo<string, short[], string>(type, deviceObject, "Write", new string[2] { "address", "value" });
		writeUInt16Func = PluginsHelper.ExecuteFuncTwo<string, ushort, string>(type, deviceObject, "Write", new string[2] { "address", "value" });
		writeUInt16ArrayFunc = PluginsHelper.ExecuteFuncTwo<string, ushort[], string>(type, deviceObject, "Write", new string[2] { "address", "value" });
		writeInt32Func = PluginsHelper.ExecuteFuncTwo<string, int, string>(type, deviceObject, "Write", new string[2] { "address", "value" });
		writeInt32ArrayFunc = PluginsHelper.ExecuteFuncTwo<string, int[], string>(type, deviceObject, "Write", new string[2] { "address", "value" });
		writeUInt32Func = PluginsHelper.ExecuteFuncTwo<string, uint, string>(type, deviceObject, "Write", new string[2] { "address", "value" });
		writeUInt32ArrayFunc = PluginsHelper.ExecuteFuncTwo<string, uint[], string>(type, deviceObject, "Write", new string[2] { "address", "value" });
		writeInt64Func = PluginsHelper.ExecuteFuncTwo<string, long, string>(type, deviceObject, "Write", new string[2] { "address", "value" });
		writeInt64ArrayFunc = PluginsHelper.ExecuteFuncTwo<string, long[], string>(type, deviceObject, "Write", new string[2] { "address", "value" });
		writeUInt64Func = PluginsHelper.ExecuteFuncTwo<string, ulong, string>(type, deviceObject, "Write", new string[2] { "address", "value" });
		writeUInt64ArrayFunc = PluginsHelper.ExecuteFuncTwo<string, ulong[], string>(type, deviceObject, "Write", new string[2] { "address", "value" });
		writeFloatFunc = PluginsHelper.ExecuteFuncTwo<string, float, string>(type, deviceObject, "Write", new string[2] { "address", "value" });
		writeFloatArrayFunc = PluginsHelper.ExecuteFuncTwo<string, float[], string>(type, deviceObject, "Write", new string[2] { "address", "value" });
		writeDoubleFunc = PluginsHelper.ExecuteFuncTwo<string, double, string>(type, deviceObject, "Write", new string[2] { "address", "value" });
		writeDoubleArrayFunc = PluginsHelper.ExecuteFuncTwo<string, double[], string>(type, deviceObject, "Write", new string[2] { "address", "value" });
		writeStringEncodingFunc = PluginsHelper.ExecuteFuncThree<string, string, Encoding, string>(type, deviceObject, "Write", new string[3] { "address", "value", "encoding" });
		readBoolSingleAsyncFunc = PluginsHelper.ExecuteFuncOne<string, Task<bool>>(type, deviceObject, "ReadBoolAsync", new string[1] { "address" });
		readByteAsyncFunc = PluginsHelper.ExecuteFuncOne<string, Task<byte>>(type, deviceObject, "ReadByteAsync", new string[1] { "address" });
		readInt16AsyncFunc = PluginsHelper.ExecuteFuncOne<string, Task<short>>(type, deviceObject, "ReadInt16Async", new string[1] { "address" });
		readInt16ArrayAsyncFunc = PluginsHelper.ExecuteFuncTwo<string, ushort, Task<short[]>>(type, deviceObject, "ReadInt16Async", new string[2] { "address", "length" });
		readUInt16AsyncFunc = PluginsHelper.ExecuteFuncOne<string, Task<ushort>>(type, deviceObject, "ReadUInt16Async", new string[1] { "address" });
		readUInt16ArrayAsyncFunc = PluginsHelper.ExecuteFuncTwo<string, ushort, Task<ushort[]>>(type, deviceObject, "ReadUInt16Async", new string[2] { "address", "length" });
		readInt32AsyncFunc = PluginsHelper.ExecuteFuncOne<string, Task<int>>(type, deviceObject, "ReadInt32Async", new string[1] { "address" });
		readInt32ArrayAsyncFunc = PluginsHelper.ExecuteFuncTwo<string, ushort, Task<int[]>>(type, deviceObject, "ReadInt32Async", new string[2] { "address", "length" });
		readUInt32AsyncFunc = PluginsHelper.ExecuteFuncOne<string, Task<uint>>(type, deviceObject, "ReadUInt32Async", new string[1] { "address" });
		readUInt32ArrayAsyncFunc = PluginsHelper.ExecuteFuncTwo<string, ushort, Task<uint[]>>(type, deviceObject, "ReadUInt32Async", new string[2] { "address", "length" });
		readInt64AsyncFunc = PluginsHelper.ExecuteFuncOne<string, Task<long>>(type, deviceObject, "ReadInt64Async", new string[1] { "address" });
		readInt64ArrayAsyncFunc = PluginsHelper.ExecuteFuncTwo<string, ushort, Task<long[]>>(type, deviceObject, "ReadInt64Async", new string[2] { "address", "length" });
		readUInt64AsyncFunc = PluginsHelper.ExecuteFuncOne<string, Task<ulong>>(type, deviceObject, "ReadUInt64Async", new string[1] { "address" });
		readUInt64ArrayAsyncFunc = PluginsHelper.ExecuteFuncTwo<string, ushort, Task<ulong[]>>(type, deviceObject, "ReadUInt64Async", new string[2] { "address", "length" });
		readFloatAsyncFunc = PluginsHelper.ExecuteFuncOne<string, Task<float>>(type, deviceObject, "ReadFloatAsync", new string[1] { "address" });
		readFloatArrayAsyncFunc = PluginsHelper.ExecuteFuncTwo<string, ushort, Task<float[]>>(type, deviceObject, "ReadFloatAsync", new string[2] { "address", "length" });
		readDoubleAsyncFunc = PluginsHelper.ExecuteFuncOne<string, Task<double>>(type, deviceObject, "ReadDoubleAsync", new string[1] { "address" });
		readDoubleArrayAsyncFunc = PluginsHelper.ExecuteFuncTwo<string, ushort, Task<double[]>>(type, deviceObject, "ReadDoubleAsync", new string[2] { "address", "length" });
		readStringAsyncFunc = PluginsHelper.ExecuteFuncThree<string, ushort, Encoding, Task<string>>(type, deviceObject, "ReadStringAsync", new string[3] { "address", "length", "encoding" });
		writeBoolSingleAsyncFunc = PluginsHelper.ExecuteFuncTwo<string, bool, Task<string>>(type, deviceObject, "WriteAsync", new string[2] { "address", "value" });
		writeByteAsyncFunc = PluginsHelper.ExecuteFuncTwo<string, byte, Task<string>>(type, deviceObject, "WriteAsync", new string[2] { "address", "value" });
		writeInt16AsyncFunc = PluginsHelper.ExecuteFuncTwo<string, short, Task<string>>(type, deviceObject, "WriteAsync", new string[2] { "address", "value" });
		writeInt16ArrayAsyncFunc = PluginsHelper.ExecuteFuncTwo<string, short[], Task<string>>(type, deviceObject, "WriteAsync", new string[2] { "address", "value" });
		writeUInt16AsyncFunc = PluginsHelper.ExecuteFuncTwo<string, ushort, Task<string>>(type, deviceObject, "WriteAsync", new string[2] { "address", "value" });
		writeUInt16ArrayAsyncFunc = PluginsHelper.ExecuteFuncTwo<string, ushort[], Task<string>>(type, deviceObject, "WriteAsync", new string[2] { "address", "value" });
		writeInt32AsyncFunc = PluginsHelper.ExecuteFuncTwo<string, int, Task<string>>(type, deviceObject, "WriteAsync", new string[2] { "address", "value" });
		writeInt32ArrayAsyncFunc = PluginsHelper.ExecuteFuncTwo<string, int[], Task<string>>(type, deviceObject, "WriteAsync", new string[2] { "address", "value" });
		writeUInt32AsyncFunc = PluginsHelper.ExecuteFuncTwo<string, uint, Task<string>>(type, deviceObject, "WriteAsync", new string[2] { "address", "value" });
		writeUInt32ArrayAsyncFunc = PluginsHelper.ExecuteFuncTwo<string, uint[], Task<string>>(type, deviceObject, "WriteAsync", new string[2] { "address", "value" });
		writeInt64AsyncFunc = PluginsHelper.ExecuteFuncTwo<string, long, Task<string>>(type, deviceObject, "WriteAsync", new string[2] { "address", "value" });
		writeInt64ArrayAsyncFunc = PluginsHelper.ExecuteFuncTwo<string, long[], Task<string>>(type, deviceObject, "WriteAsync", new string[2] { "address", "value" });
		writeUInt64AsyncFunc = PluginsHelper.ExecuteFuncTwo<string, ulong, Task<string>>(type, deviceObject, "WriteAsync", new string[2] { "address", "value" });
		writeUInt64ArrayAsyncFunc = PluginsHelper.ExecuteFuncTwo<string, ulong[], Task<string>>(type, deviceObject, "WriteAsync", new string[2] { "address", "value" });
		writeFloatAsyncFunc = PluginsHelper.ExecuteFuncTwo<string, float, Task<string>>(type, deviceObject, "WriteAsync", new string[2] { "address", "value" });
		writeFloatArrayAsyncFunc = PluginsHelper.ExecuteFuncTwo<string, float[], Task<string>>(type, deviceObject, "WriteAsync", new string[2] { "address", "value" });
		writeDoubleAsyncFunc = PluginsHelper.ExecuteFuncTwo<string, double, Task<string>>(type, deviceObject, "WriteAsync", new string[2] { "address", "value" });
		writeDoubleArrayAsyncFunc = PluginsHelper.ExecuteFuncTwo<string, double[], Task<string>>(type, deviceObject, "WriteAsync", new string[2] { "address", "value" });
		writeStringEncodingAsyncFunc = PluginsHelper.ExecuteFuncThree<string, string, Encoding, Task<string>>(type, deviceObject, "WriteAsync", new string[3] { "address", "value", "encoding" });
	}

	protected override INetMessage GetNewNetMessage()
	{
		return pluginMessage;
	}

	protected override OperateResult InitializationOnConnect()
	{
		if (InitializationOnConnectFunc != null)
		{
			try
			{
				int num = InitializationOnConnectFunc();
				if (num != 0)
				{
					return new OperateResult(num, "InitializationOnConnect failed: " + num);
				}
			}
			catch (Exception ex)
			{
				return new OperateResult("InitializationOnConnect failed: " + ex.Message);
			}
		}
		return base.InitializationOnConnect();
	}

	protected override async Task<OperateResult> InitializationOnConnectAsync()
	{
		if (InitializationOnConnectFunc != null)
		{
			try
			{
				InitializationOnConnectFunc();
			}
			catch (Exception ex)
			{
				Exception ex2 = ex;
				return new OperateResult("InitializationOnConnect failed: " + ex2.Message);
			}
		}
		return await base.InitializationOnConnectAsync().ConfigureAwait(continueOnCapturedContext: false);
	}

	protected override OperateResult<List<byte[]>> BuildDeviceReadCommand(string address, ushort length, bool isBit)
	{
		if (BuildDeviceReadCommandFunc != null)
		{
			try
			{
				return OperateResult.CreateSuccessResult(BuildDeviceReadCommandFunc(address, length, isBit));
			}
			catch (Exception ex)
			{
				return new OperateResult<List<byte[]>>(ex.Message);
			}
		}
		return base.BuildDeviceReadCommand(address, length, isBit);
	}

	protected override OperateResult<byte[]> BuildDeviceWriteCommand(string address, bool[] value)
	{
		if (BuildDeviceWriteBoolCommandFunc != null)
		{
			try
			{
				return OperateResult.CreateSuccessResult(BuildDeviceWriteBoolCommandFunc(address, value));
			}
			catch (Exception ex)
			{
				return new OperateResult<byte[]>(ex.Message);
			}
		}
		return base.BuildDeviceWriteCommand(address, value);
	}

	protected override OperateResult<byte[]> BuildDeviceWriteCommand(string address, byte[] value)
	{
		if (BuildDeviceWriteByteCommandFunc != null)
		{
			try
			{
				return OperateResult.CreateSuccessResult(BuildDeviceWriteByteCommandFunc(address, value));
			}
			catch (Exception ex)
			{
				return new OperateResult<byte[]>(ex.Message);
			}
		}
		return base.BuildDeviceWriteCommand(address, value);
	}

	protected override OperateResult CheckDeviceResponse(string address, byte[] buildCommand, byte[] content)
	{
		if (CheckDeviceResponseFunc != null)
		{
			try
			{
				string text = CheckDeviceResponseFunc(address, buildCommand, content);
				if (string.IsNullOrEmpty(text))
				{
					return OperateResult.CreateSuccessResult();
				}
				return new OperateResult<byte[]>(text);
			}
			catch (Exception ex)
			{
				return new OperateResult<byte[]>("CheckDeviceResponse failed: " + ex.Message);
			}
		}
		return base.CheckDeviceResponse(address, buildCommand, content);
	}

	protected override OperateResult<byte[]> ParseDeviceReadResult(string address, byte[] buildCommand, byte[] content)
	{
		if (ParseDeviceReadResultFunc != null)
		{
			try
			{
				return OperateResult.CreateSuccessResult(ParseDeviceReadResultFunc(address, buildCommand, content));
			}
			catch (Exception ex)
			{
				return new OperateResult<byte[]>("ParseDeviceReadResult failed: " + ex.Message);
			}
		}
		return base.ParseDeviceReadResult(address, buildCommand, content);
	}

	protected override OperateResult<bool[]> ParseDeviceReadBoolResult(string address, byte[] buildCommand, byte[] content, ushort length)
	{
		if (ParseDeviceReadBoolResultFunc != null)
		{
			try
			{
				return OperateResult.CreateSuccessResult(ParseDeviceReadBoolResultFunc(address, buildCommand, content, length));
			}
			catch (Exception ex)
			{
				return new OperateResult<bool[]>("ParseDeviceReadBoolResult failed: " + ex.Message);
			}
		}
		return base.ParseDeviceReadBoolResult(address, buildCommand, content, length);
	}

	[HslMqttApi("ReadByteArray", "")]
	public override OperateResult<byte[]> Read(string address, ushort length)
	{
		if (ReadBytesFunc != null)
		{
			try
			{
				return OperateResult.CreateSuccessResult(ReadBytesFunc(address, length));
			}
			catch (Exception ex)
			{
				return CreateFailedResult<byte[]>(ex);
			}
		}
		return base.Read(address, length);
	}

	[HslMqttApi("ReadBoolArray", "")]
	public override OperateResult<bool[]> ReadBool(string address, ushort length)
	{
		if (ReadBoolFunc != null)
		{
			try
			{
				return OperateResult.CreateSuccessResult(ReadBoolFunc(address, length));
			}
			catch (Exception ex)
			{
				return CreateFailedResult<bool[]>(ex);
			}
		}
		return base.ReadBool(address, length);
	}

	[HslMqttApi("WriteByteArray", "")]
	public override OperateResult Write(string address, byte[] value)
	{
		if (WriteBytesFunc != null)
		{
			try
			{
				string text = WriteBytesFunc(address, value);
				if (string.IsNullOrEmpty(text))
				{
					return OperateResult.CreateSuccessResult();
				}
				return new OperateResult(text);
			}
			catch (Exception ex)
			{
				return CreateFailedResult<byte[]>(ex);
			}
		}
		return base.Write(address, value);
	}

	[HslMqttApi("WriteBoolArray", "")]
	public override OperateResult Write(string address, bool[] value)
	{
		if (WriteBoolFunc != null)
		{
			try
			{
				string text = WriteBoolFunc(address, value);
				if (string.IsNullOrEmpty(text))
				{
					return OperateResult.CreateSuccessResult();
				}
				return new OperateResult(text);
			}
			catch (Exception ex)
			{
				return CreateFailedResult<bool[]>(ex);
			}
		}
		return base.Write(address, value);
	}

	private byte[] ReadFromCoreServerHelper(byte[] send, bool hasResponseData, bool usePackAndUnpack)
	{
		OperateResult<byte[]> operateResult = ReadFromCoreServer(CommunicationPipe, send, hasResponseData, usePackAndUnpack);
		if (operateResult.IsSuccess)
		{
			return operateResult.Content;
		}
		throw new Exception(operateResult.Message);
	}

	private byte[] ReadFromCoreServerHelper2(byte[] send)
	{
		OperateResult<byte[]> operateResult = ReadFromCoreServer(send);
		if (operateResult.IsSuccess)
		{
			return operateResult.Content;
		}
		throw new Exception(operateResult.Message);
	}

	private async Task<byte[]> ReadFromCoreServerHelperAsync(byte[] send, bool hasResponseData, bool usePackAndUnpack)
	{
		OperateResult<byte[]> read = await ReadFromCoreServerAsync(CommunicationPipe, send, hasResponseData, usePackAndUnpack).ConfigureAwait(continueOnCapturedContext: false);
		if (read.IsSuccess)
		{
			return read.Content;
		}
		throw new Exception(read.Message);
	}

	private async Task<byte[]> ReadFromCoreServerHelper2Async(byte[] send)
	{
		OperateResult<byte[]> read = await ReadFromCoreServerAsync(send).ConfigureAwait(continueOnCapturedContext: false);
		if (read.IsSuccess)
		{
			return read.Content;
		}
		throw new Exception(read.Message);
	}

	private OperateResult<T> CreateFailedResult<T>(Exception ex)
	{
		return new OperateResult<T>(ex.Message);
	}

	[HslMqttApi("ReadBool", "")]
	public override OperateResult<bool> ReadBool(string address)
	{
		if (readBoolSingleFunc != null)
		{
			try
			{
				return OperateResult.CreateSuccessResult(readBoolSingleFunc(address));
			}
			catch (Exception ex)
			{
				return CreateFailedResult<bool>(ex);
			}
		}
		return base.ReadBool(address);
	}

	[HslMqttApi("ReadByte", "")]
	/// <summary>
	/// 读取一个字节的数据，插件设备如果有定义ReadByte(string address)方法，则会直接调用该方法，否则会调用Read(string address, ushort length)方法来读取
	/// </summary>
	/// <param name="address">设备的地址信息</param>
	/// <returns>包含byte数据的操作结果</returns>
	public OperateResult<byte> ReadByte(string address)
	{
		if (readByteFunc != null)
		{
			try
			{
				return OperateResult.CreateSuccessResult(readByteFunc(address));
			}
			catch (Exception ex)
			{
				return CreateFailedResult<byte>(ex);
			}
		}
		OperateResult<byte[]> operateResult = Read(address, 1);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte>(operateResult);
		}
		if (operateResult.Content != null && operateResult.Content.Length != 0)
		{
			return OperateResult.CreateSuccessResult(operateResult.Content[0]);
		}
		return new OperateResult<byte>("Read byte failed, no data returned.");
	}

	[HslMqttApi("ReadInt16", "")]
	public override OperateResult<short> ReadInt16(string address)
	{
		if (readInt16Func != null)
		{
			try
			{
				return OperateResult.CreateSuccessResult(readInt16Func(address));
			}
			catch (Exception ex)
			{
				return CreateFailedResult<short>(ex);
			}
		}
		return base.ReadInt16(address);
	}

	[HslMqttApi("ReadInt16Array", "")]
	public override OperateResult<short[]> ReadInt16(string address, ushort length)
	{
		if (readInt16ArrayFunc != null)
		{
			try
			{
				return OperateResult.CreateSuccessResult(readInt16ArrayFunc(address, length));
			}
			catch (Exception ex)
			{
				return CreateFailedResult<short[]>(ex);
			}
		}
		return base.ReadInt16(address, length);
	}

	[HslMqttApi("ReadUInt16", "")]
	public override OperateResult<ushort> ReadUInt16(string address)
	{
		if (readUInt16Func != null)
		{
			try
			{
				return OperateResult.CreateSuccessResult(readUInt16Func(address));
			}
			catch (Exception ex)
			{
				return CreateFailedResult<ushort>(ex);
			}
		}
		return base.ReadUInt16(address);
	}

	[HslMqttApi("ReadUInt16Array", "")]
	public override OperateResult<ushort[]> ReadUInt16(string address, ushort length)
	{
		if (readUInt16ArrayFunc != null)
		{
			try
			{
				return OperateResult.CreateSuccessResult(readUInt16ArrayFunc(address, length));
			}
			catch (Exception ex)
			{
				return CreateFailedResult<ushort[]>(ex);
			}
		}
		return base.ReadUInt16(address, length);
	}

	[HslMqttApi("ReadInt32", "")]
	public override OperateResult<int> ReadInt32(string address)
	{
		if (readInt32Func != null)
		{
			try
			{
				return OperateResult.CreateSuccessResult(readInt32Func(address));
			}
			catch (Exception ex)
			{
				return CreateFailedResult<int>(ex);
			}
		}
		return base.ReadInt32(address);
	}

	[HslMqttApi("ReadInt32Array", "")]
	public override OperateResult<int[]> ReadInt32(string address, ushort length)
	{
		if (readInt32ArrayFunc != null)
		{
			try
			{
				return OperateResult.CreateSuccessResult(readInt32ArrayFunc(address, length));
			}
			catch (Exception ex)
			{
				return CreateFailedResult<int[]>(ex);
			}
		}
		return base.ReadInt32(address, length);
	}

	[HslMqttApi("ReadUInt32", "")]
	public override OperateResult<uint> ReadUInt32(string address)
	{
		if (readUInt32Func != null)
		{
			try
			{
				return OperateResult.CreateSuccessResult(readUInt32Func(address));
			}
			catch (Exception ex)
			{
				return CreateFailedResult<uint>(ex);
			}
		}
		return base.ReadUInt32(address);
	}

	[HslMqttApi("ReadUInt32Array", "")]
	public override OperateResult<uint[]> ReadUInt32(string address, ushort length)
	{
		if (readUInt32ArrayFunc != null)
		{
			try
			{
				return OperateResult.CreateSuccessResult(readUInt32ArrayFunc(address, length));
			}
			catch (Exception ex)
			{
				return CreateFailedResult<uint[]>(ex);
			}
		}
		return base.ReadUInt32(address, length);
	}

	[HslMqttApi("ReadInt64", "")]
	public override OperateResult<long> ReadInt64(string address)
	{
		if (readInt64Func != null)
		{
			try
			{
				return OperateResult.CreateSuccessResult(readInt64Func(address));
			}
			catch (Exception ex)
			{
				return CreateFailedResult<long>(ex);
			}
		}
		return base.ReadInt64(address);
	}

	[HslMqttApi("ReadInt64Array", "")]
	public override OperateResult<long[]> ReadInt64(string address, ushort length)
	{
		if (readInt64ArrayFunc != null)
		{
			try
			{
				return OperateResult.CreateSuccessResult(readInt64ArrayFunc(address, length));
			}
			catch (Exception ex)
			{
				return CreateFailedResult<long[]>(ex);
			}
		}
		return base.ReadInt64(address, length);
	}

	[HslMqttApi("ReadUInt64", "")]
	public override OperateResult<ulong> ReadUInt64(string address)
	{
		if (readUInt64Func != null)
		{
			try
			{
				return OperateResult.CreateSuccessResult(readUInt64Func(address));
			}
			catch (Exception ex)
			{
				return CreateFailedResult<ulong>(ex);
			}
		}
		return base.ReadUInt64(address);
	}

	[HslMqttApi("ReadUInt64Array", "")]
	public override OperateResult<ulong[]> ReadUInt64(string address, ushort length)
	{
		if (readUInt64ArrayFunc != null)
		{
			try
			{
				return OperateResult.CreateSuccessResult(readUInt64ArrayFunc(address, length));
			}
			catch (Exception ex)
			{
				return CreateFailedResult<ulong[]>(ex);
			}
		}
		return base.ReadUInt64(address, length);
	}

	[HslMqttApi("ReadFloat", "")]
	public override OperateResult<float> ReadFloat(string address)
	{
		if (readFloatFunc != null)
		{
			try
			{
				return OperateResult.CreateSuccessResult(readFloatFunc(address));
			}
			catch (Exception ex)
			{
				return CreateFailedResult<float>(ex);
			}
		}
		return base.ReadFloat(address);
	}

	[HslMqttApi("ReadFloatArray", "")]
	public override OperateResult<float[]> ReadFloat(string address, ushort length)
	{
		if (readFloatArrayFunc != null)
		{
			try
			{
				return OperateResult.CreateSuccessResult(readFloatArrayFunc(address, length));
			}
			catch (Exception ex)
			{
				return CreateFailedResult<float[]>(ex);
			}
		}
		return base.ReadFloat(address, length);
	}

	[HslMqttApi("ReadDouble", "")]
	public override OperateResult<double> ReadDouble(string address)
	{
		if (readDoubleFunc != null)
		{
			try
			{
				return OperateResult.CreateSuccessResult(readDoubleFunc(address));
			}
			catch (Exception ex)
			{
				return CreateFailedResult<double>(ex);
			}
		}
		return base.ReadDouble(address);
	}

	[HslMqttApi("ReadDoubleArray", "")]
	public override OperateResult<double[]> ReadDouble(string address, ushort length)
	{
		if (readDoubleArrayFunc != null)
		{
			try
			{
				return OperateResult.CreateSuccessResult(readDoubleArrayFunc(address, length));
			}
			catch (Exception ex)
			{
				return CreateFailedResult<double[]>(ex);
			}
		}
		return base.ReadDouble(address, length);
	}

	public override OperateResult<string> ReadString(string address, ushort length, Encoding encoding)
	{
		if (readStringFunc != null)
		{
			try
			{
				return OperateResult.CreateSuccessResult(readStringFunc(address, length, encoding));
			}
			catch (Exception ex)
			{
				return CreateFailedResult<string>(ex);
			}
		}
		return base.ReadString(address, length, encoding);
	}

	private OperateResult WriteValueHelper<T>(string address, T value, Func<string, T, string> writeFunc)
	{
		if (writeFunc != null)
		{
			try
			{
				string text = writeFunc(address, value);
				if (string.IsNullOrEmpty(text))
				{
					return OperateResult.CreateSuccessResult();
				}
				return new OperateResult(text);
			}
			catch (Exception ex)
			{
				return CreateFailedResult<T>(ex);
			}
		}
		return new OperateResult("Write function is not defined.");
	}

	[HslMqttApi("WriteBool", "")]
	public override OperateResult Write(string address, bool value)
	{
		if (writeBoolSingleFunc != null)
		{
			return WriteValueHelper(address, value, writeBoolSingleFunc);
		}
		return base.Write(address, value);
	}

	[HslMqttApi("WriteByte", "")]
	/// <summary>
	/// 写入一个字节的数据，插件设备如果有定义WriteByte(string address, byte value)方法，则会直接调用该方法，否则会调用Write(string address, byte[] value)方法来写入
	/// </summary>
	/// <param name="address">要写入的地址</param>
	/// <param name="value">要写入的字节值</param>
	/// <returns>操作结果</returns>
	public OperateResult Write(string address, byte value)
	{
		if (writeByteFunc != null)
		{
			return WriteValueHelper(address, value, writeByteFunc);
		}
		return Write(address, new byte[1] { value });
	}

	[HslMqttApi("WriteInt16", "")]
	public override OperateResult Write(string address, short value)
	{
		if (writeInt16Func != null)
		{
			return WriteValueHelper(address, value, writeInt16Func);
		}
		return base.Write(address, value);
	}

	[HslMqttApi("WriteInt16Array", "")]
	public override OperateResult Write(string address, short[] values)
	{
		if (writeInt16ArrayFunc != null)
		{
			return WriteValueHelper(address, values, writeInt16ArrayFunc);
		}
		return base.Write(address, values);
	}

	[HslMqttApi("WriteUInt16", "")]
	public override OperateResult Write(string address, ushort value)
	{
		if (writeUInt16Func != null)
		{
			return WriteValueHelper(address, value, writeUInt16Func);
		}
		return base.Write(address, value);
	}

	[HslMqttApi("WriteUInt16Array", "")]
	public override OperateResult Write(string address, ushort[] values)
	{
		if (writeUInt16ArrayFunc != null)
		{
			return WriteValueHelper(address, values, writeUInt16ArrayFunc);
		}
		return base.Write(address, values);
	}

	[HslMqttApi("WriteInt32", "")]
	public override OperateResult Write(string address, int value)
	{
		if (writeInt32Func != null)
		{
			return WriteValueHelper(address, value, writeInt32Func);
		}
		return base.Write(address, value);
	}

	[HslMqttApi("WriteInt32Array", "")]
	public override OperateResult Write(string address, int[] values)
	{
		if (writeInt32ArrayFunc != null)
		{
			return WriteValueHelper(address, values, writeInt32ArrayFunc);
		}
		return base.Write(address, values);
	}

	[HslMqttApi("WriteUInt32", "")]
	public override OperateResult Write(string address, uint value)
	{
		if (writeUInt32Func != null)
		{
			return WriteValueHelper(address, value, writeUInt32Func);
		}
		return base.Write(address, value);
	}

	[HslMqttApi("WriteUInt32Array", "")]
	public override OperateResult Write(string address, uint[] values)
	{
		if (writeUInt32ArrayFunc != null)
		{
			return WriteValueHelper(address, values, writeUInt32ArrayFunc);
		}
		return base.Write(address, values);
	}

	[HslMqttApi("WriteInt64", "")]
	public override OperateResult Write(string address, long value)
	{
		if (writeInt64Func != null)
		{
			return WriteValueHelper(address, value, writeInt64Func);
		}
		return base.Write(address, value);
	}

	[HslMqttApi("WriteInt64Array", "")]
	public override OperateResult Write(string address, long[] values)
	{
		if (writeInt64ArrayFunc != null)
		{
			return WriteValueHelper(address, values, writeInt64ArrayFunc);
		}
		return base.Write(address, values);
	}

	[HslMqttApi("WriteUInt64", "")]
	public override OperateResult Write(string address, ulong value)
	{
		if (writeUInt64Func != null)
		{
			return WriteValueHelper(address, value, writeUInt64Func);
		}
		return base.Write(address, value);
	}

	[HslMqttApi("WriteUInt64Array", "")]
	public override OperateResult Write(string address, ulong[] values)
	{
		if (writeUInt64ArrayFunc != null)
		{
			return WriteValueHelper(address, values, writeUInt64ArrayFunc);
		}
		return base.Write(address, values);
	}

	[HslMqttApi("WriteFloat", "")]
	public override OperateResult Write(string address, float value)
	{
		if (writeFloatFunc != null)
		{
			return WriteValueHelper(address, value, writeFloatFunc);
		}
		return base.Write(address, value);
	}

	[HslMqttApi("WriteFloatArray", "")]
	public override OperateResult Write(string address, float[] values)
	{
		if (writeFloatArrayFunc != null)
		{
			return WriteValueHelper(address, values, writeFloatArrayFunc);
		}
		return base.Write(address, values);
	}

	[HslMqttApi("WriteDouble", "")]
	public override OperateResult Write(string address, double value)
	{
		if (writeDoubleFunc != null)
		{
			return WriteValueHelper(address, value, writeDoubleFunc);
		}
		return base.Write(address, value);
	}

	[HslMqttApi("WriteDoubleArray", "")]
	public override OperateResult Write(string address, double[] values)
	{
		if (writeDoubleArrayFunc != null)
		{
			return WriteValueHelper(address, values, writeDoubleArrayFunc);
		}
		return base.Write(address, values);
	}

	public override OperateResult Write(string address, string value, Encoding encoding)
	{
		if (writeStringEncodingFunc != null)
		{
			try
			{
				string text = writeStringEncodingFunc(address, value, encoding);
				if (string.IsNullOrEmpty(text))
				{
					return OperateResult.CreateSuccessResult();
				}
				return new OperateResult(text);
			}
			catch (Exception ex)
			{
				return CreateFailedResult<string>(ex);
			}
		}
		return base.Write(address, value, encoding);
	}

	public override async Task<OperateResult<byte[]>> ReadAsync(string address, ushort length)
	{
		if (ReadBytesAsyncFunc != null)
		{
			try
			{
				return OperateResult.CreateSuccessResult(await ReadBytesAsyncFunc(address, length).ConfigureAwait(continueOnCapturedContext: false));
			}
			catch (Exception ex)
			{
				return CreateFailedResult<byte[]>(ex);
			}
		}
		return await base.ReadAsync(address, length);
	}

	public override async Task<OperateResult<bool[]>> ReadBoolAsync(string address, ushort length)
	{
		if (ReadBoolAsyncFunc != null)
		{
			try
			{
				return OperateResult.CreateSuccessResult(await ReadBoolAsyncFunc(address, length).ConfigureAwait(continueOnCapturedContext: false));
			}
			catch (Exception ex)
			{
				return CreateFailedResult<bool[]>(ex);
			}
		}
		return await base.ReadBoolAsync(address, length);
	}

	private async Task<OperateResult> WriteValueAsyncHelper<T>(string address, T value, Func<string, T, Task<string>> writeFunc)
	{
		if (writeFunc != null)
		{
			try
			{
				string write = await writeFunc(address, value).ConfigureAwait(continueOnCapturedContext: false);
				if (string.IsNullOrEmpty(write))
				{
					return OperateResult.CreateSuccessResult();
				}
				return new OperateResult(write);
			}
			catch (Exception ex)
			{
				return CreateFailedResult<T>(ex);
			}
		}
		return new OperateResult("Write function is not defined.");
	}

	public override async Task<OperateResult> WriteAsync(string address, byte[] value)
	{
		if (WriteBytesAsyncFunc != null)
		{
			return await WriteValueAsyncHelper(address, value, WriteBytesAsyncFunc).ConfigureAwait(continueOnCapturedContext: false);
		}
		return await base.WriteAsync(address, value).ConfigureAwait(continueOnCapturedContext: false);
	}

	public override async Task<OperateResult> WriteAsync(string address, bool[] value)
	{
		if (WriteBoolAsyncFunc != null)
		{
			return await WriteValueAsyncHelper(address, value, WriteBoolAsyncFunc).ConfigureAwait(continueOnCapturedContext: false);
		}
		return await base.WriteAsync(address, value).ConfigureAwait(continueOnCapturedContext: false);
	}

	public override async Task<OperateResult<bool>> ReadBoolAsync(string address)
	{
		if (readBoolSingleAsyncFunc != null)
		{
			try
			{
				return OperateResult.CreateSuccessResult(await readBoolSingleAsyncFunc(address).ConfigureAwait(continueOnCapturedContext: false));
			}
			catch (Exception ex)
			{
				return CreateFailedResult<bool>(ex);
			}
		}
		return await base.ReadBoolAsync(address).ConfigureAwait(continueOnCapturedContext: false);
	}

	/// <summary>
	/// 读取一个字节的数据，插件设备如果有定义ReadByte(string address)方法，则会直接调用该方法，否则会调用Read(string address, ushort length)方法来读取
	/// </summary>
	/// <param name="address">设备的地址信息</param>
	/// <returns>包含byte数据的操作结果</returns>
	public async Task<OperateResult<byte>> ReadByteAsync(string address)
	{
		if (readByteAsyncFunc != null)
		{
			try
			{
				return OperateResult.CreateSuccessResult(await readByteAsyncFunc(address).ConfigureAwait(continueOnCapturedContext: false));
			}
			catch (Exception ex)
			{
				return CreateFailedResult<byte>(ex);
			}
		}
		OperateResult<byte[]> read = Read(address, 1);
		if (!read.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte>(read);
		}
		if (read.Content != null && read.Content.Length != 0)
		{
			return OperateResult.CreateSuccessResult(read.Content[0]);
		}
		return new OperateResult<byte>("Read byte failed, no data returned.");
	}

	public override async Task<OperateResult<short>> ReadInt16Async(string address)
	{
		if (readInt16AsyncFunc != null)
		{
			try
			{
				return OperateResult.CreateSuccessResult(await readInt16AsyncFunc(address).ConfigureAwait(continueOnCapturedContext: false));
			}
			catch (Exception ex)
			{
				return CreateFailedResult<short>(ex);
			}
		}
		return await base.ReadInt16Async(address).ConfigureAwait(continueOnCapturedContext: false);
	}

	public override async Task<OperateResult<short[]>> ReadInt16Async(string address, ushort length)
	{
		if (readInt16ArrayAsyncFunc != null)
		{
			try
			{
				return OperateResult.CreateSuccessResult(await readInt16ArrayAsyncFunc(address, length).ConfigureAwait(continueOnCapturedContext: false));
			}
			catch (Exception ex)
			{
				return CreateFailedResult<short[]>(ex);
			}
		}
		return await base.ReadInt16Async(address, length).ConfigureAwait(continueOnCapturedContext: false);
	}

	public override async Task<OperateResult<ushort>> ReadUInt16Async(string address)
	{
		if (readUInt16AsyncFunc != null)
		{
			try
			{
				return OperateResult.CreateSuccessResult(await readUInt16AsyncFunc(address).ConfigureAwait(continueOnCapturedContext: false));
			}
			catch (Exception ex)
			{
				return CreateFailedResult<ushort>(ex);
			}
		}
		return await base.ReadUInt16Async(address).ConfigureAwait(continueOnCapturedContext: false);
	}

	public override async Task<OperateResult<ushort[]>> ReadUInt16Async(string address, ushort length)
	{
		if (readUInt16ArrayAsyncFunc != null)
		{
			try
			{
				return OperateResult.CreateSuccessResult(await readUInt16ArrayAsyncFunc(address, length).ConfigureAwait(continueOnCapturedContext: false));
			}
			catch (Exception ex)
			{
				return CreateFailedResult<ushort[]>(ex);
			}
		}
		return await base.ReadUInt16Async(address, length).ConfigureAwait(continueOnCapturedContext: false);
	}

	public override async Task<OperateResult<int>> ReadInt32Async(string address)
	{
		if (readInt32AsyncFunc != null)
		{
			try
			{
				return OperateResult.CreateSuccessResult(await readInt32AsyncFunc(address).ConfigureAwait(continueOnCapturedContext: false));
			}
			catch (Exception ex)
			{
				return CreateFailedResult<int>(ex);
			}
		}
		return await base.ReadInt32Async(address).ConfigureAwait(continueOnCapturedContext: false);
	}

	public override async Task<OperateResult<int[]>> ReadInt32Async(string address, ushort length)
	{
		if (readInt32ArrayAsyncFunc != null)
		{
			try
			{
				return OperateResult.CreateSuccessResult(await readInt32ArrayAsyncFunc(address, length).ConfigureAwait(continueOnCapturedContext: false));
			}
			catch (Exception ex)
			{
				return CreateFailedResult<int[]>(ex);
			}
		}
		return await base.ReadInt32Async(address, length).ConfigureAwait(continueOnCapturedContext: false);
	}

	public override async Task<OperateResult<uint>> ReadUInt32Async(string address)
	{
		if (readUInt32AsyncFunc != null)
		{
			try
			{
				return OperateResult.CreateSuccessResult(await readUInt32AsyncFunc(address).ConfigureAwait(continueOnCapturedContext: false));
			}
			catch (Exception ex)
			{
				return CreateFailedResult<uint>(ex);
			}
		}
		return await base.ReadUInt32Async(address).ConfigureAwait(continueOnCapturedContext: false);
	}

	public override async Task<OperateResult<uint[]>> ReadUInt32Async(string address, ushort length)
	{
		if (readUInt32ArrayAsyncFunc != null)
		{
			try
			{
				return OperateResult.CreateSuccessResult(await readUInt32ArrayAsyncFunc(address, length).ConfigureAwait(continueOnCapturedContext: false));
			}
			catch (Exception ex)
			{
				return CreateFailedResult<uint[]>(ex);
			}
		}
		return await base.ReadUInt32Async(address, length).ConfigureAwait(continueOnCapturedContext: false);
	}

	public override async Task<OperateResult<long>> ReadInt64Async(string address)
	{
		if (readInt64AsyncFunc != null)
		{
			try
			{
				return OperateResult.CreateSuccessResult(await readInt64AsyncFunc(address).ConfigureAwait(continueOnCapturedContext: false));
			}
			catch (Exception ex)
			{
				return CreateFailedResult<long>(ex);
			}
		}
		return await base.ReadInt64Async(address).ConfigureAwait(continueOnCapturedContext: false);
	}

	public override async Task<OperateResult<long[]>> ReadInt64Async(string address, ushort length)
	{
		if (readInt64ArrayAsyncFunc != null)
		{
			try
			{
				return OperateResult.CreateSuccessResult(await readInt64ArrayAsyncFunc(address, length).ConfigureAwait(continueOnCapturedContext: false));
			}
			catch (Exception ex)
			{
				return CreateFailedResult<long[]>(ex);
			}
		}
		return await base.ReadInt64Async(address, length).ConfigureAwait(continueOnCapturedContext: false);
	}

	public override async Task<OperateResult<ulong>> ReadUInt64Async(string address)
	{
		if (readUInt64AsyncFunc != null)
		{
			try
			{
				return OperateResult.CreateSuccessResult(await readUInt64AsyncFunc(address).ConfigureAwait(continueOnCapturedContext: false));
			}
			catch (Exception ex)
			{
				return CreateFailedResult<ulong>(ex);
			}
		}
		return await base.ReadUInt64Async(address).ConfigureAwait(continueOnCapturedContext: false);
	}

	public override async Task<OperateResult<ulong[]>> ReadUInt64Async(string address, ushort length)
	{
		if (readUInt64ArrayAsyncFunc != null)
		{
			try
			{
				return OperateResult.CreateSuccessResult(await readUInt64ArrayAsyncFunc(address, length).ConfigureAwait(continueOnCapturedContext: false));
			}
			catch (Exception ex)
			{
				return CreateFailedResult<ulong[]>(ex);
			}
		}
		return await base.ReadUInt64Async(address, length).ConfigureAwait(continueOnCapturedContext: false);
	}

	public override async Task<OperateResult<float>> ReadFloatAsync(string address)
	{
		if (readFloatAsyncFunc != null)
		{
			try
			{
				return OperateResult.CreateSuccessResult(await readFloatAsyncFunc(address).ConfigureAwait(continueOnCapturedContext: false));
			}
			catch (Exception ex)
			{
				return CreateFailedResult<float>(ex);
			}
		}
		return await base.ReadFloatAsync(address).ConfigureAwait(continueOnCapturedContext: false);
	}

	public override async Task<OperateResult<float[]>> ReadFloatAsync(string address, ushort length)
	{
		if (readFloatArrayAsyncFunc != null)
		{
			try
			{
				return OperateResult.CreateSuccessResult(await readFloatArrayAsyncFunc(address, length).ConfigureAwait(continueOnCapturedContext: false));
			}
			catch (Exception ex)
			{
				return CreateFailedResult<float[]>(ex);
			}
		}
		return await base.ReadFloatAsync(address, length).ConfigureAwait(continueOnCapturedContext: false);
	}

	public override async Task<OperateResult<double>> ReadDoubleAsync(string address)
	{
		if (readDoubleAsyncFunc != null)
		{
			try
			{
				return OperateResult.CreateSuccessResult(await readDoubleAsyncFunc(address).ConfigureAwait(continueOnCapturedContext: false));
			}
			catch (Exception ex)
			{
				return CreateFailedResult<double>(ex);
			}
		}
		return await base.ReadDoubleAsync(address).ConfigureAwait(continueOnCapturedContext: false);
	}

	public override async Task<OperateResult<double[]>> ReadDoubleAsync(string address, ushort length)
	{
		if (readDoubleArrayAsyncFunc != null)
		{
			try
			{
				return OperateResult.CreateSuccessResult(await readDoubleArrayAsyncFunc(address, length).ConfigureAwait(continueOnCapturedContext: false));
			}
			catch (Exception ex)
			{
				return CreateFailedResult<double[]>(ex);
			}
		}
		return await base.ReadDoubleAsync(address, length).ConfigureAwait(continueOnCapturedContext: false);
	}

	public override async Task<OperateResult<string>> ReadStringAsync(string address, ushort length, Encoding encoding)
	{
		if (readStringAsyncFunc != null)
		{
			try
			{
				return OperateResult.CreateSuccessResult(await readStringAsyncFunc(address, length, encoding).ConfigureAwait(continueOnCapturedContext: false));
			}
			catch (Exception ex)
			{
				return CreateFailedResult<string>(ex);
			}
		}
		return await base.ReadStringAsync(address, length, encoding).ConfigureAwait(continueOnCapturedContext: false);
	}

	public override async Task<OperateResult> WriteAsync(string address, bool value)
	{
		if (writeBoolSingleAsyncFunc != null)
		{
			return await WriteValueAsyncHelper(address, value, writeBoolSingleAsyncFunc).ConfigureAwait(continueOnCapturedContext: false);
		}
		return await base.WriteAsync(address, value).ConfigureAwait(continueOnCapturedContext: false);
	}

	/// <summary>
	/// 写入一个字节的数据，插件设备如果有定义WriteByte(string address, byte value)方法，则会直接调用该方法，否则会调用Write(string address, byte[] value)方法来写入
	/// </summary>
	/// <param name="address">要写入的地址</param>
	/// <param name="value">要写入的字节值</param>
	/// <returns>操作结果</returns>
	public async Task<OperateResult> WriteAsync(string address, byte value)
	{
		if (writeByteAsyncFunc != null)
		{
			return await WriteValueAsyncHelper(address, value, writeByteAsyncFunc).ConfigureAwait(continueOnCapturedContext: false);
		}
		return await WriteAsync(address, new byte[1] { value }).ConfigureAwait(continueOnCapturedContext: false);
	}

	public override async Task<OperateResult> WriteAsync(string address, short value)
	{
		if (writeInt16AsyncFunc != null)
		{
			return await WriteValueAsyncHelper(address, value, writeInt16AsyncFunc).ConfigureAwait(continueOnCapturedContext: false);
		}
		return await base.WriteAsync(address, value).ConfigureAwait(continueOnCapturedContext: false);
	}

	public override async Task<OperateResult> WriteAsync(string address, short[] values)
	{
		if (writeInt16ArrayAsyncFunc != null)
		{
			return await WriteValueAsyncHelper(address, values, writeInt16ArrayAsyncFunc).ConfigureAwait(continueOnCapturedContext: false);
		}
		return await base.WriteAsync(address, values).ConfigureAwait(continueOnCapturedContext: false);
	}

	public override async Task<OperateResult> WriteAsync(string address, ushort value)
	{
		if (writeUInt16AsyncFunc != null)
		{
			return await WriteValueAsyncHelper(address, value, writeUInt16AsyncFunc).ConfigureAwait(continueOnCapturedContext: false);
		}
		return await base.WriteAsync(address, value).ConfigureAwait(continueOnCapturedContext: false);
	}

	public override async Task<OperateResult> WriteAsync(string address, ushort[] values)
	{
		if (writeUInt16ArrayAsyncFunc != null)
		{
			return await WriteValueAsyncHelper(address, values, writeUInt16ArrayAsyncFunc).ConfigureAwait(continueOnCapturedContext: false);
		}
		return await base.WriteAsync(address, values).ConfigureAwait(continueOnCapturedContext: false);
	}

	public override async Task<OperateResult> WriteAsync(string address, int value)
	{
		if (writeInt32AsyncFunc != null)
		{
			return await WriteValueAsyncHelper(address, value, writeInt32AsyncFunc).ConfigureAwait(continueOnCapturedContext: false);
		}
		return await base.WriteAsync(address, value).ConfigureAwait(continueOnCapturedContext: false);
	}

	public override async Task<OperateResult> WriteAsync(string address, int[] values)
	{
		if (writeInt32ArrayAsyncFunc != null)
		{
			return await WriteValueAsyncHelper(address, values, writeInt32ArrayAsyncFunc).ConfigureAwait(continueOnCapturedContext: false);
		}
		return await base.WriteAsync(address, values).ConfigureAwait(continueOnCapturedContext: false);
	}

	public override async Task<OperateResult> WriteAsync(string address, uint value)
	{
		if (writeUInt32AsyncFunc != null)
		{
			return await WriteValueAsyncHelper(address, value, writeUInt32AsyncFunc).ConfigureAwait(continueOnCapturedContext: false);
		}
		return await base.WriteAsync(address, value).ConfigureAwait(continueOnCapturedContext: false);
	}

	public override async Task<OperateResult> WriteAsync(string address, uint[] values)
	{
		if (writeUInt32ArrayAsyncFunc != null)
		{
			return await WriteValueAsyncHelper(address, values, writeUInt32ArrayAsyncFunc).ConfigureAwait(continueOnCapturedContext: false);
		}
		return await base.WriteAsync(address, values).ConfigureAwait(continueOnCapturedContext: false);
	}

	public override async Task<OperateResult> WriteAsync(string address, long value)
	{
		if (writeInt64AsyncFunc != null)
		{
			return await WriteValueAsyncHelper(address, value, writeInt64AsyncFunc).ConfigureAwait(continueOnCapturedContext: false);
		}
		return await base.WriteAsync(address, value).ConfigureAwait(continueOnCapturedContext: false);
	}

	public override async Task<OperateResult> WriteAsync(string address, long[] values)
	{
		if (writeInt64ArrayAsyncFunc != null)
		{
			return await WriteValueAsyncHelper(address, values, writeInt64ArrayAsyncFunc).ConfigureAwait(continueOnCapturedContext: false);
		}
		return await base.WriteAsync(address, values).ConfigureAwait(continueOnCapturedContext: false);
	}

	public override async Task<OperateResult> WriteAsync(string address, ulong value)
	{
		if (writeUInt64AsyncFunc != null)
		{
			return await WriteValueAsyncHelper(address, value, writeUInt64AsyncFunc).ConfigureAwait(continueOnCapturedContext: false);
		}
		return await base.WriteAsync(address, value).ConfigureAwait(continueOnCapturedContext: false);
	}

	public override async Task<OperateResult> WriteAsync(string address, ulong[] values)
	{
		if (writeUInt64ArrayAsyncFunc != null)
		{
			return await WriteValueAsyncHelper(address, values, writeUInt64ArrayAsyncFunc).ConfigureAwait(continueOnCapturedContext: false);
		}
		return await base.WriteAsync(address, values).ConfigureAwait(continueOnCapturedContext: false);
	}

	public override async Task<OperateResult> WriteAsync(string address, float value)
	{
		if (writeFloatAsyncFunc != null)
		{
			return await WriteValueAsyncHelper(address, value, writeFloatAsyncFunc).ConfigureAwait(continueOnCapturedContext: false);
		}
		return await base.WriteAsync(address, value).ConfigureAwait(continueOnCapturedContext: false);
	}

	public override async Task<OperateResult> WriteAsync(string address, float[] values)
	{
		if (writeFloatArrayAsyncFunc != null)
		{
			return await WriteValueAsyncHelper(address, values, writeFloatArrayAsyncFunc).ConfigureAwait(continueOnCapturedContext: false);
		}
		return await base.WriteAsync(address, values).ConfigureAwait(continueOnCapturedContext: false);
	}

	public override async Task<OperateResult> WriteAsync(string address, double value)
	{
		if (writeDoubleAsyncFunc != null)
		{
			return await WriteValueAsyncHelper(address, value, writeDoubleAsyncFunc).ConfigureAwait(continueOnCapturedContext: false);
		}
		return await base.WriteAsync(address, value).ConfigureAwait(continueOnCapturedContext: false);
	}

	public override async Task<OperateResult> WriteAsync(string address, double[] values)
	{
		if (writeDoubleArrayAsyncFunc != null)
		{
			return await WriteValueAsyncHelper(address, values, writeDoubleArrayAsyncFunc).ConfigureAwait(continueOnCapturedContext: false);
		}
		return await base.WriteAsync(address, values).ConfigureAwait(continueOnCapturedContext: false);
	}

	public override async Task<OperateResult> WriteAsync(string address, string value, Encoding encoding)
	{
		if (writeStringEncodingAsyncFunc != null)
		{
			try
			{
				string write = await writeStringEncodingAsyncFunc(address, value, encoding).ConfigureAwait(continueOnCapturedContext: false);
				if (string.IsNullOrEmpty(write))
				{
					return OperateResult.CreateSuccessResult();
				}
				return new OperateResult(write);
			}
			catch (Exception ex)
			{
				return CreateFailedResult<string>(ex);
			}
		}
		return await base.WriteAsync(address, value, encoding).ConfigureAwait(continueOnCapturedContext: false);
	}

	public override OperateResult ConnectServer()
	{
		if (connectServerFunc != null)
		{
			try
			{
				string text = connectServerFunc();
				if (string.IsNullOrEmpty(text))
				{
					return OperateResult.CreateSuccessResult();
				}
				return new OperateResult(text);
			}
			catch (Exception ex)
			{
				return CreateFailedResult<string>(ex);
			}
		}
		return base.ConnectServer();
	}

	public override async Task<OperateResult> ConnectServerAsync()
	{
		if (connectServerAsyncFunc != null)
		{
			try
			{
				string result = await connectServerAsyncFunc().ConfigureAwait(continueOnCapturedContext: false);
				if (string.IsNullOrEmpty(result))
				{
					return OperateResult.CreateSuccessResult();
				}
				return new OperateResult(result);
			}
			catch (Exception ex)
			{
				return CreateFailedResult<string>(ex);
			}
		}
		return await base.ConnectServerAsync().ConfigureAwait(continueOnCapturedContext: false);
	}

	public override OperateResult ConnectClose()
	{
		if (connectClose != null)
		{
			try
			{
				string text = connectClose();
				if (string.IsNullOrEmpty(text))
				{
					return OperateResult.CreateSuccessResult();
				}
				return new OperateResult(text);
			}
			catch (Exception ex)
			{
				return CreateFailedResult<string>(ex);
			}
		}
		return base.ConnectClose();
	}

	public override async Task<OperateResult> ConnectCloseAsync()
	{
		if (connectCloseAsyncFunc != null)
		{
			try
			{
				string result = await connectCloseAsyncFunc().ConfigureAwait(continueOnCapturedContext: false);
				if (string.IsNullOrEmpty(result))
				{
					return OperateResult.CreateSuccessResult();
				}
				return new OperateResult(result);
			}
			catch (Exception ex)
			{
				return CreateFailedResult<string>(ex);
			}
		}
		return await base.ConnectCloseAsync().ConfigureAwait(continueOnCapturedContext: false);
	}

	/// <summary>
	/// 设置插件的参数对象，插件设备如果有定义SetParameters(object obj)方法，则会直接调用该方法，成功返回空字符串，否则返回错误信息
	/// </summary>
	/// <param name="obj">要设置的参数对象</param>
	/// <returns>操作结果字符串</returns>
	public string SetParameters(object obj)
	{
		if (setParametersMethod != null)
		{
			object obj2 = setParametersMethod.Invoke(pluginObject, new object[1] { obj });
			if (obj2 == null)
			{
				return string.Empty;
			}
			return obj2.ToString();
		}
		return string.Empty;
	}

	/// <summary>
	/// 获取当前的插件对象
	/// </summary>
	public dynamic GetPluginObject()
	{
		return pluginObject;
	}

	/// <summary>
	/// 从一个文件里加载设备的插件信息，指定插件文件路径，设备名称，设置参数的方法
	/// </summary>
	/// <param name="file">插件文件的完整路径</param>
	/// <param name="deviceName">设备名称</param>
	/// <param name="parameters">参数字典</param>
	/// <returns>创建的参数对象信息</returns>
	public static DevicePluginNet CreateFromFile(string file, string deviceName = null, Dictionary<string, object> parameters = null)
	{
		Assembly assembly = Assembly.LoadFrom(file);
		Type type = assembly.GetTypes().FirstOrDefault((Type m) => m.Name == "PluginsHelper");
		if (type != null)
		{
			object obj = type.GetMethod("DeviceDefinitions")?.Invoke(null, null);
			if (obj != null)
			{
				if (obj.GetType() == typeof(object[]))
				{
					return CreateDevicePluginNet(obj as object[], deviceName, parameters);
				}
				if (obj.GetType() == typeof(List<object>))
				{
					return CreateDevicePluginNet(obj as List<object>, deviceName, parameters);
				}
			}
		}
		return null;
	}

	private static DevicePluginNet CreateDevicePluginNet(IEnumerable<object> list, string deviceName = null, Dictionary<string, object> parameters = null)
	{
		foreach (object item in list)
		{
			bool success = false;
			PluginsDeviceDefinition pluginsDeviceDefinition = new PluginsDeviceDefinition(item);
			if (!string.IsNullOrEmpty(deviceName) && deviceName == pluginsDeviceDefinition.DeviceName)
			{
				success = true;
			}
			if (string.IsNullOrEmpty(deviceName))
			{
				success = true;
			}
			if (!success)
			{
				continue;
			}
			object obj = null;
			if (pluginsDeviceDefinition.DevicePara != null)
			{
				obj = pluginsDeviceDefinition.DevicePara.Assembly.CreateInstance(pluginsDeviceDefinition.DevicePara.FullName, ignoreCase: true, BindingFlags.Default, null, new object[0], null, null);
			}
			object deviceObject = pluginsDeviceDefinition.DeviceObject.Assembly.CreateInstance(pluginsDeviceDefinition.DeviceObject.FullName, ignoreCase: true, BindingFlags.Default, null, new object[0], null, null);
			DevicePluginNet devicePluginNet = new DevicePluginNet(deviceObject);
			if (obj != null)
			{
				if (parameters != null)
				{
					foreach (KeyValuePair<string, object> parameter in parameters)
					{
						PropertyInfo property = obj.GetType().GetProperty(parameter.Key);
						if (property == null)
						{
							property = obj.GetType().GetProperty(parameter.Key, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);
						}
						if (property == null)
						{
							throw new Exception("The parameter object does not contain the property '" + parameter.Key + "'.");
						}
						if (property != null && property.CanWrite)
						{
							property.SetValue(obj, parameter.Value, null);
						}
					}
				}
				string text = devicePluginNet.SetParameters(obj);
				if (!string.IsNullOrEmpty(text))
				{
					throw new Exception(text);
				}
			}
			return devicePluginNet;
		}
		return null;
	}
}
