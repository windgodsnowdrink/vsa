// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace HslCommunication.Core.Plugin;

/// <summary>
/// 插件相关的辅助方法
/// </summary>
public class PluginsHelper
{
	public static T GetObjectProperty<T>(Type pluginType, object obj, string name)
	{
		PropertyInfo property = pluginType.GetProperty(name);
		if (property == null)
		{
			return default(T);
		}
		object value = property.GetValue(obj, null);
		if (value == null)
		{
			return default(T);
		}
		try
		{
			return (T)value;
		}
		catch
		{
			return default(T);
		}
	}

	public static T GetObjectProperty<T>(object obj, string name)
	{
		Type type = obj.GetType();
		return GetObjectProperty<T>(type, obj, name);
	}

	public static Func<TResult> ExecuteFuncZero<TResult>(Type type, object deviceObject, string methodName, params string[] paraNames)
	{
		MethodInfo method = type.GetMethod(methodName, new Type[0]);
		if (method == null)
		{
			return null;
		}
		ConstantExpression instance = Expression.Constant(deviceObject, type);
		Expression body = Expression.Call(instance, method);
		Expression<Func<TResult>> expression = Expression.Lambda<Func<TResult>>(body, Array.Empty<ParameterExpression>());
		return expression.Compile();
	}

	public static Func<T1, TResult> ExecuteFuncOne<T1, TResult>(Type type, object deviceObject, string methodName, params string[] paraNames)
	{
		MethodInfo method = type.GetMethod(methodName, new Type[1] { typeof(T1) });
		if (method == null)
		{
			return null;
		}
		ConstantExpression instance = Expression.Constant(deviceObject, type);
		ParameterExpression parameterExpression = Expression.Parameter(typeof(T1), paraNames[0]);
		Expression body = Expression.Call(instance, method, parameterExpression);
		Expression<Func<T1, TResult>> expression = Expression.Lambda<Func<T1, TResult>>(body, new ParameterExpression[1] { parameterExpression });
		return expression.Compile();
	}

	public static Func<T1, T2, TResult> ExecuteFuncTwo<T1, T2, TResult>(Type type, object deviceObject, string methodName, params string[] paraNames)
	{
		MethodInfo method = type.GetMethod(methodName, new Type[2]
		{
			typeof(T1),
			typeof(T2)
		});
		if (method == null)
		{
			return null;
		}
		ConstantExpression instance = Expression.Constant(deviceObject, type);
		ParameterExpression parameterExpression = Expression.Parameter(typeof(T1), paraNames[0]);
		ParameterExpression parameterExpression2 = Expression.Parameter(typeof(T2), paraNames[1]);
		Expression body = Expression.Call(instance, method, parameterExpression, parameterExpression2);
		Expression<Func<T1, T2, TResult>> expression = Expression.Lambda<Func<T1, T2, TResult>>(body, new ParameterExpression[2] { parameterExpression, parameterExpression2 });
		return expression.Compile();
	}

	public static Func<T1, T2, T3, TResult> ExecuteFuncThree<T1, T2, T3, TResult>(Type type, object deviceObject, string methodName, params string[] paraNames)
	{
		MethodInfo method = type.GetMethod(methodName, new Type[3]
		{
			typeof(T1),
			typeof(T2),
			typeof(T3)
		});
		if (method == null)
		{
			return null;
		}
		ConstantExpression instance = Expression.Constant(deviceObject, type);
		ParameterExpression parameterExpression = Expression.Parameter(typeof(T1), paraNames[0]);
		ParameterExpression parameterExpression2 = Expression.Parameter(typeof(T2), paraNames[1]);
		ParameterExpression parameterExpression3 = Expression.Parameter(typeof(T3), paraNames[2]);
		Expression body = Expression.Call(instance, method, parameterExpression, parameterExpression2, parameterExpression3);
		Expression<Func<T1, T2, T3, TResult>> expression = Expression.Lambda<Func<T1, T2, T3, TResult>>(body, new ParameterExpression[3] { parameterExpression, parameterExpression2, parameterExpression3 });
		return expression.Compile();
	}

	public static Func<T1, T2, T3, T4, TResult> ExecuteFuncFour<T1, T2, T3, T4, TResult>(Type type, object deviceObject, string methodName, params string[] paraNames)
	{
		MethodInfo method = type.GetMethod(methodName, new Type[4]
		{
			typeof(T1),
			typeof(T2),
			typeof(T3),
			typeof(T4)
		});
		if (method == null)
		{
			return null;
		}
		ConstantExpression instance = Expression.Constant(deviceObject, type);
		ParameterExpression parameterExpression = Expression.Parameter(typeof(T1), paraNames[0]);
		ParameterExpression parameterExpression2 = Expression.Parameter(typeof(T2), paraNames[1]);
		ParameterExpression parameterExpression3 = Expression.Parameter(typeof(T3), paraNames[2]);
		ParameterExpression parameterExpression4 = Expression.Parameter(typeof(T4), paraNames[3]);
		Expression body = Expression.Call(instance, method, parameterExpression, parameterExpression2, parameterExpression3, parameterExpression4);
		Expression<Func<T1, T2, T3, T4, TResult>> expression = Expression.Lambda<Func<T1, T2, T3, T4, TResult>>(body, new ParameterExpression[4] { parameterExpression, parameterExpression2, parameterExpression3, parameterExpression4 });
		return expression.Compile();
	}
}
