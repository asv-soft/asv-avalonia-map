using System;
using System.Threading.Tasks;
using Asv.Avalonia.Map.Demo.Menu.MenuItems;
using Asv.Common;
using DynamicData;
using Material.Icons;
using ReactiveUI.Fody.Helpers;

namespace Asv.Avalonia.Map.Demo.Models;

public class ShellPage : ViewModelBase,IShellPage
{
    protected ShellPage(Uri uri) : base(uri)
    {
        HeaderItemsSource = new SourceCache<IHeaderMenuItem, Uri>(x=>x.Id).DisposeItWith(Disposable);

       
    }
    protected ShellPage(string uri) : base(uri)
    {
        HeaderItemsSource = new SourceCache<IHeaderMenuItem, Uri>(x=>x.Id).DisposeItWith(Disposable);
       
    }
    
    [Reactive]
    public MaterialIconKind Icon { get; set; }
    [Reactive]
    public string Title { get; set; }
    
    protected ISourceCache<IHeaderMenuItem,Uri> HeaderItemsSource { get; }
    
    public IObservable<IChangeSet<IHeaderMenuItem, Uri>> HeaderItems => HeaderItemsSource.Connect();
    

    public virtual void SetArgs(Uri link)
    {
            
    }

    public virtual Task<bool> TryClose()
    {
        return Task.FromResult(true);
    }
}