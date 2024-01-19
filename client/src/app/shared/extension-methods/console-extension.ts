interface Console {
    success(message: any): void;
    customize(message?: any, ...options: any[]): void;
}