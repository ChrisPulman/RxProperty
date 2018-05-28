using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Reactive.Bindings.Helpers;
using System;
using UWP.TodoMVVM.Models;
using UWP.TodoMVVM.Services;

namespace UWP.TodoMVVM.ViewModels
{
    internal class MainPageViewModel
    {
        public MainPageViewModel(TodoManager todoManager, TodoService todoService)
        {
            this.TodoManager = todoManager;
            this.TodoService = todoService;

            this.AddTodoItemCommand = this.InputTodoItem
                .Select(x => x.HasErrors)
                .Switch()
                .Select(x => !x)
                .ToRxCommand();
            this.AddTodoItemCommand
                .Select(_ => this.InputTodoItem.Value)
                .Subscribe(x =>
                {
                    this.TodoService.AddTodoItem(x);
                    this.InputTodoItem.Value = new TodoItemViewModel();
                });

            this.TodoItems = this.TodoManager
                .ObserveProperty(x => x.CurrentView)
                .Do(_ => this.TodoItems?.Value?.Dispose())
                .Select(x => x.ToReadOnlyReactiveCollection(y => new TodoItemViewModel(y)))
                .ToReactiveProperty();

            this.ShowAllCommand = this.TodoManager.ObserveProperty(x => x.ViewType)
                .Select(x => x != ViewType.All)
                .ToRxCommand();
            this.ShowAllCommand.Subscribe(_ => this.TodoService.SetAllCurrentView());

            this.ShowActiveCommand = this.TodoManager.ObserveProperty(x => x.ViewType)
                .Select(x => x != ViewType.Active)
                .ToRxCommand();
            this.ShowActiveCommand.Subscribe(_ => this.TodoService.SetActiveCurrentView());

            this.ShowCompletedCommand = this.TodoManager.ObserveProperty(x => x.ViewType)
                .Select(x => x != ViewType.Completed)
                .ToRxCommand();
            this.ShowCompletedCommand.Subscribe(_ => this.TodoService.SetCompletedCurrentView());

            this.ClearCompletedTodoItemCommand = this.TodoManager
                .ChangedAsObservable
                .Select(x => x.Any(y => y.Completed))
                .ToRxCommand();
            this.ClearCompletedTodoItemCommand.Subscribe(_ => this.TodoService.ClearCompletedTodoItem());
        }

        public RxCommand AddTodoItemCommand { get; }

        public RxCommand ClearCompletedTodoItemCommand { get; }

        public ReactiveProperty<TodoItemViewModel> InputTodoItem { get; } = new ReactiveProperty<TodoItemViewModel>(new TodoItemViewModel());

        public RxCommand ShowActiveCommand { get; }

        public RxCommand ShowAllCommand { get; }

        public RxCommand ShowCompletedCommand { get; }

        public ReactiveProperty<ReadOnlyReactiveCollection<TodoItemViewModel>> TodoItems { get; }

        private TodoManager TodoManager { get; }

        private TodoService TodoService { get; }

        public void ChangeStateAll(bool completed)
        {
            this.TodoService.ChangeStateAll(completed);
        }
    }
}