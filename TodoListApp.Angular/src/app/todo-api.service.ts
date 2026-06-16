import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

export interface TodoList {
  id: number;
  title: string;
  description: string;
}

export interface TodoTask {
  id: number;
  title: string;
  description: string;
  createdAt: string;
  dueDate?: string | null;
  status: number;
  assigneeId: string;
  todoListId: number;
  categoryId?: number | null;
  categoryName?: string | null;
}

export interface Category {
  id: number;
  name: string;
}

export interface PagedResult<T> {
  items: T[];
  pageNumber: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}

export interface TaskQuery {
  searchText?: string;
  category?: string;
  pageNumber: number;
  pageSize: number;
}

export interface AuthResponse {
  token: string;
  email: string;
}

@Injectable({ providedIn: 'root' })
export class TodoApiService {
  private readonly apiBaseUrl = 'http://localhost:5000/api';

  constructor(private readonly http: HttpClient) {}

  login(email: string, password: string): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiBaseUrl}/auth/login`, { email, password });
  }

  register(email: string, password: string): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiBaseUrl}/auth/register`, { email, password });
  }

  getTodoLists(): Observable<TodoList[]> {
    return this.http.get<TodoList[]>(`${this.apiBaseUrl}/TodoList`);
  }

  createTodoList(list: Partial<TodoList>): Observable<TodoList> {
    return this.http.post<TodoList>(`${this.apiBaseUrl}/TodoList`, list);
  }

  getCategories(): Observable<Category[]> {
    return this.http.get<Category[]>(`${this.apiBaseUrl}/category`);
  }

  createCategory(name: string): Observable<Category> {
    return this.http.post<Category>(`${this.apiBaseUrl}/category`, { name });
  }

  getTasks(todoListId: number, query: TaskQuery): Observable<PagedResult<TodoTask>> {
    let params = new HttpParams()
      .set('pageNumber', query.pageNumber)
      .set('pageSize', query.pageSize);

    if (query.searchText) {
      params = params.set('searchText', query.searchText);
    }

    if (query.category) {
      params = params.set('category', query.category);
    }

    return this.http.get<PagedResult<TodoTask>>(
      `${this.apiBaseUrl}/todolists/${todoListId}/tasks`,
      { params },
    );
  }

  createTask(todoListId: number, task: Partial<TodoTask>): Observable<TodoTask> {
    return this.http.post<TodoTask>(`${this.apiBaseUrl}/todolists/${todoListId}/tasks`, task);
  }

  updateTask(todoListId: number, taskId: number, task: Partial<TodoTask>): Observable<void> {
    return this.http.put<void>(`${this.apiBaseUrl}/todolists/${todoListId}/tasks/${taskId}`, task);
  }

  deleteTask(todoListId: number, taskId: number): Observable<void> {
    return this.http.delete<void>(`${this.apiBaseUrl}/todolists/${todoListId}/tasks/${taskId}`);
  }
}
