export function Input({ className = '', ...props }) {
  return (
    <input
      className={`w-full rounded-md border border-[var(--color-border)] px-3 py-2 text-sm
        focus:outline-none focus:ring-2 focus:ring-[var(--color-primary)]/30 focus:border-[var(--color-primary)]
        disabled:opacity-50 disabled:cursor-not-allowed ${className}`}
      {...props}
    />
  );
}

export function Label({ children, htmlFor, className = '' }) {
  return (
    <label htmlFor={htmlFor} className={`block text-sm font-medium text-gray-700 mb-1 ${className}`}>
      {children}
    </label>
  );
}
