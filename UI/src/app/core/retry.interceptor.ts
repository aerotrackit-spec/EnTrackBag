import {
  HttpErrorResponse,
  HttpInterceptorFn,
} from "@angular/common/http";
import { timer, throwError } from "rxjs";
import { retry } from "rxjs/operators";
import { environment } from "../../environments/environment";

function isRetryable(error: unknown): boolean {
  if (!(error instanceof HttpErrorResponse)) return false;

  return [0, 408, 429, 500, 502, 503, 504].includes(error.status);
}

export const retryInterceptor: HttpInterceptorFn = (req, next) => {
  return next(req).pipe(
    retry({
      count: environment.apiRetryCount,
      delay: (error, retryAttempt) => {
        if (!isRetryable(error)) {
          return throwError(() => error);
        }

        return timer(environment.apiRetryDelayMilliseconds * retryAttempt);
      },
    }),
  );
};
