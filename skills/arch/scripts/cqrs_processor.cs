#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web
#:package System.Threading.Tasks.Dataflow@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net10.0

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Cqrs.Processor
{
    /// <summary>
    /// 命令接口
    /// 所有命令都需要实现此接口
    /// </summary>
    public interface ICommand
    {}

    /// <summary>
    /// 创建待办事项命令
    /// </summary>
    /// <param name="Title">待办事项标题</param>
    public record CreateTodoCommand(string Title) : ICommand;

    /// <summary>
    /// 更新待办事项命令
    /// </summary>
    /// <param name="Id">待办事项ID</param>
    /// <param name="IsCompleted">是否完成</param>
    public record UpdateTodoCommand(int Id, bool IsCompleted) : ICommand;

    /// <summary>
    /// CQRS命令处理器
    /// 使用Dataflow实现命令的并行处理
    /// </summary>
    public class CqrsProcessor
    {
        /// <summary>
        /// 命令广播块
        /// 用于将命令广播到多个处理程序
        /// </summary>
        private readonly BroadcastBlock<ICommand> _commandBroadcaster;

        /// <summary>
        /// 命令处理器字典
        /// 键为命令类型，值为对应的处理块
        /// </summary>
        private readonly Dictionary<Type, ITargetBlock<ICommand>> _handlers = new();

        /// <summary>
        /// 构造函数
        /// 初始化命令处理器并注册默认命令处理程序
        /// </summary>
        public CqrsProcessor()
        {
            // 初始化广播块，使用命令本身作为输出
            _commandBroadcaster = new BroadcastBlock<ICommand>(cmd => cmd);

            // 注册命令处理器
            RegisterHandler<CreateTodoCommand>(new ActionBlock<CreateTodoCommand>(HandleCreate));
            RegisterHandler<UpdateTodoCommand>(new ActionBlock<UpdateTodoCommand>(HandleUpdate));
        }

        /// <summary>
        /// 注册命令处理器
        /// </summary>
        /// <typeparam name="T">命令类型</typeparam>
        /// <param name="handler">命令处理块</param>
        private void RegisterHandler<T>(ITargetBlock<T> handler) where T : ICommand
        {
            // 创建类型转换块，将ICommand转换为具体的命令类型
            var adapter = new TransformBlock<ICommand, T>(cmd => (T)cmd);
            // 将转换块链接到处理块
            adapter.LinkTo(handler);
            // 将转换块添加到处理器字典
            _handlers[typeof(T)] = adapter;

            // 将广播块链接到转换块
            _commandBroadcaster.LinkTo(adapter, cmd => cmd is T);
        }

        /// <summary>
        /// 发送命令
        /// </summary>
        /// <param name="command">命令对象</param>
        /// <returns>任务</returns>
        public async Task SendAsync(ICommand command)
        {
            await _commandBroadcaster.SendAsync(command);
        }

        /// <summary>
        /// 处理创建待办事项命令
        /// </summary>
        /// <param name="command">创建待办事项命令</param>
        /// <returns>任务</returns>
        private async Task HandleCreate(CreateTodoCommand command)
        {
            // 模拟处理创建命令
            await Task.Delay(100);
            Console.WriteLine($"创建待办事项: {command.Title}");
        }

        /// <summary>
        /// 处理更新待办事项命令
        /// </summary>
        /// <param name="command">更新待办事项命令</param>
        /// <returns>任务</returns>
        private async Task HandleUpdate(UpdateTodoCommand command)
        {
            // 模拟处理更新命令
            await Task.Delay(100);
            Console.WriteLine($"更新待办事项: ID={command.Id}, 完成状态={command.IsCompleted}");
        }
    }

    /// <summary>
    /// 主程序
    /// </summary>
    public class Program
    {
        /// <summary>
        /// 主入口点
        /// </summary>
        /// <param name="args">命令行参数</param>
        /// <returns>任务</returns>
        public static async Task Main(string[] args)
        {
            // 创建CQRS处理器
            var processor = new CqrsProcessor();

            // 发送创建命令
            await processor.SendAsync(new CreateTodoCommand("学习CQRS模式"));

            // 发送更新命令
            await processor.SendAsync(new UpdateTodoCommand(1, true));

            // 等待处理完成
            await Task.Delay(1000);

            Console.WriteLine("命令处理完成");
        }
    }
}
