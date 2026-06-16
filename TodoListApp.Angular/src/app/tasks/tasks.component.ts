import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../auth/auth.service';
import { Category, TodoApiService, TodoList, TodoTask } from '../todo-api.service';

interface TaskForm {
  id?: number;
  title: string;
  description: string;
  dueDate: string;
  status: number;
  categoryId: number | null;
}

@Component({
  selector: 'app-tasks',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './tasks.component.html',
  styleUrl: './tasks.component.css',
})
export class TasksComponent implements OnInit {
  lists: TodoList[] = [];
  selectedListId = 0;
  tasks: TodoTask[] = [];
  categories: Category[] = [];
  newCategoryName = '';

  errorMessage = '';
  isLoading = false;

  searchText = '';
  selectedCategory = '';
  pageNumber = 1;
  pageSize = 5;
  totalPages = 0;
  totalCount = 0;

  taskForm: TaskForm = this.createEmptyTaskForm();

  constructor(
    private readonly api: TodoApiService,
    private readonly auth: AuthService,
    private readonly router: Router,
  ) {}

  get currentUser(): string {
    return this.auth.currentUser;
  }

  ngOnInit(): void {
    this.loadLists();
    this.loadCategories();
  }

  logout(): void {
    this.auth.logout();
    this.router.navigate(['/login']);
  }

  loadLists(): void {
    this.api.getTodoLists().subscribe({
      next: (lists) => {
        this.lists = lists;
        this.selectedListId = lists[0]?.id ?? 0;

        if (this.selectedListId) {
          this.loadTasks();
        }
      },
      error: () => {
        this.errorMessage = 'Cannot load todo lists. Start TodoListApp.WebApi first.';
      },
    });
  }

  createDefaultList(): void {
    this.api.createTodoList({
      title: 'My Tasks',
      description: 'Default task list',
    }).subscribe({
      next: (list) => {
        this.lists = [list, ...this.lists];
        this.selectedListId = list.id;
        this.loadTasks();
      },
      error: () => {
        this.errorMessage = 'Cannot create default list.';
      },
    });
  }

  changeList(): void {
    this.pageNumber = 1;
    this.loadTasks();
  }

  applyFilters(): void {
    this.pageNumber = 1;
    this.loadTasks();
  }

  resetFilters(): void {
    this.searchText = '';
    this.selectedCategory = '';
    this.pageNumber = 1;
    this.loadTasks();
  }

  goToPage(page: number): void {
    if (page < 1 || page > this.totalPages || page === this.pageNumber) {
      return;
    }

    this.pageNumber = page;
    this.loadTasks();
  }

  saveTask(): void {
    if (!this.selectedListId || !this.taskForm.title.trim()) {
      return;
    }

    const task = {
      id: this.taskForm.id ?? 0,
      title: this.taskForm.title.trim(),
      description: this.taskForm.description.trim(),
      dueDate: this.taskForm.dueDate || null,
      status: Number(this.taskForm.status),
      assigneeId: this.currentUser,
      todoListId: this.selectedListId,
      categoryId: this.taskForm.categoryId,
    };

    if (this.taskForm.id) {
      const taskId = this.taskForm.id;
      this.api.updateTask(this.selectedListId, taskId, task).subscribe({
        next: () => {
          this.resetTaskForm();
          this.loadTasks();
        },
        error: () => {
          this.errorMessage = 'Cannot save task.';
        },
      });
      return;
    }

    this.api.createTask(this.selectedListId, task).subscribe({
      next: () => {
        this.resetTaskForm();
        this.loadTasks();
      },
      error: () => {
        this.errorMessage = 'Cannot save task.';
      },
    });
  }

  editTask(task: TodoTask): void {
    this.taskForm = {
      id: task.id,
      title: task.title,
      description: task.description,
      dueDate: task.dueDate ? task.dueDate.substring(0, 10) : '',
      status: task.status,
      categoryId: task.categoryId ?? null,
    };
  }

  deleteTask(task: TodoTask): void {
    this.api.deleteTask(task.todoListId, task.id).subscribe({
      next: () => this.loadTasks(),
      error: () => {
        this.errorMessage = 'Cannot delete task.';
      },
    });
  }

  addCategory(): void {
    const name = this.newCategoryName.trim();
    if (!name) {
      return;
    }

    this.api.createCategory(name).subscribe({
      next: () => {
        this.newCategoryName = '';
        this.loadCategories();
      },
      error: () => {
        this.errorMessage = 'Cannot create category.';
      },
    });
  }

  resetTaskForm(): void {
    this.taskForm = this.createEmptyTaskForm();
  }

  statusLabel(status: number): string {
    switch (status) {
      case 1:
        return 'In Progress';
      case 2:
        return 'Completed';
      default:
        return 'Not Started';
    }
  }

  pageNumbers(): number[] {
    return Array.from({ length: this.totalPages }, (_, index) => index + 1);
  }

  private loadTasks(): void {
    if (!this.selectedListId) {
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';

    this.api.getTasks(this.selectedListId, {
      searchText: this.searchText,
      category: this.selectedCategory,
      pageNumber: this.pageNumber,
      pageSize: this.pageSize,
    }).subscribe({
      next: (result) => {
        this.tasks = result.items;
        this.pageNumber = result.pageNumber;
        this.pageSize = result.pageSize;
        this.totalCount = result.totalCount;
        this.totalPages = result.totalPages;
        this.isLoading = false;
      },
      error: () => {
        this.errorMessage = 'Cannot load tasks.';
        this.isLoading = false;
      },
    });
  }

  private loadCategories(): void {
    this.api.getCategories().subscribe({
      next: (categories) => {
        this.categories = categories;
      },
    });
  }

  private createEmptyTaskForm(): TaskForm {
    return {
      title: '',
      description: '',
      dueDate: '',
      status: 0,
      categoryId: null,
    };
  }
}
