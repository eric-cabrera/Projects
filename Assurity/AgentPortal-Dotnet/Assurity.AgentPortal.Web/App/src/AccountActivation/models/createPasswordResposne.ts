export interface CreatePasswordResponse {
  valid: boolean;
  activationAttempts: number;
  message?: string;
}
