#:sdk Microsoft.NET.Sdk
#:package ReactiveUI@18.3.1
#:package System.Reactive@6.0.0
#:property TargetFramework net8.0
#:property Nullable enable
#:property ImplicitUsings enable

using ReactiveUI;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;

// ViewModel基类
public abstract class ReactiveViewModelBase : ReactiveObject, IDisposable
{
    protected readonly CompositeDisposable _disposables = new();
    private readonly ObjectPool<CompositeDisposable> _disposablePool;
    
    protected ReactiveViewModelBase(ObjectPool<CompositeDisposable> disposablePool)
    {
        _disposablePool = disposablePool;
        _disposables = _disposablePool.Get();
    }
    
    protected void AddDisposable(IDisposable disposable)
    {
        _disposables.Add(disposable);
    }
    
    public void Dispose()
    {
        _disposablePool.Return(_disposables);
    }
}

// 示例ViewModel
public class UserProfileViewModel : ReactiveViewModelBase
{
    private string _name;
    private int _age;
    
    public string Name
    {
        get => _name;
        set => this.RaiseAndSetIfChanged(ref _name, value);
    }
    
    public int Age
    {
        get => _age;
        set => this.RaiseAndSetIfChanged(ref _age, value);
    }
    
    public ReactiveCommand<Unit, Unit> SaveCommand { get; }
    public Interaction<string, bool> ConfirmInteraction { get; } = new();
    
    public UserProfileViewModel(ObjectPool<CompositeDisposable> disposablePool) 
        : base(disposablePool)
    {
        // 命令绑定
        var canSave = this.WhenAnyValue(
            x => x.Name,
            x => x.Age,
            (name, age) => !string.IsNullOrEmpty(name) && age > 0);
            
        SaveCommand = ReactiveCommand.CreateFromTask(SaveAsync, canSave);
        
        // 订阅命令执行
        SaveCommand
            .Throttle(TimeSpan.FromMilliseconds(500))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(_ => Console.WriteLine("Saved!"))
            .DisposeWith(_disposables);
            
        // 组合流示例
        this.WhenAnyValue(x => x.Name, x => x.Age)
            .Throttle(TimeSpan.FromMilliseconds(300))
            .Where(t => !string.IsNullOrEmpty(t.Item1) && t.Item2 > 0)
            .Select(t => $"{t.Item1} ({t.Item2})")
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(Console.WriteLine)
            .DisposeWith(_disposables);
    }
    
    private async Task SaveAsync()
    {
        var confirmed = await ConfirmInteraction.Handle("Are you sure?");
        if (confirmed)
        {
            // 保存逻辑
        }
    }
}

// DI扩展
public static class ReactiveExtensions
{
    public static IServiceCollection AddReactiveServices(this IServiceCollection services)
    {
        // 对象池配置
        services.AddSingleton<ObjectPool<CompositeDisposable>>(sp => 
            new DefaultObjectPool<CompositeDisposable>(new CompositeDisposablePooledObjectPolicy(), 100));
            
        // 注册ViewModel
        services.AddTransient<UserProfileViewModel>();
        
        return services;
    }
}

// 在WPF/Maui/Avalonia中使用
// public partial class UserProfileView : ReactiveWindow<UserProfileViewModel>
// {
//     public UserProfileView()
//     {
//         InitializeComponent();
//         ViewModel = new UserProfileViewModel();
//         
//         // 绑定命令
//         this.BindCommand(ViewModel, vm => vm.SaveCommand, v => v.SaveButton);
//         
//         // 绑定属性
//         this.Bind(ViewModel, vm => vm.Name, v => v.NameTextBox.Text);
//         this.Bind(ViewModel, vm => vm.Age, v => v.AgeTextBox.Text);
//     }
// }