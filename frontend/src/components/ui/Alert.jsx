const variants = {
  info: 'bg-blue-50 border-blue-200 text-blue-800',
  success: 'bg-green-50 border-green-200 text-green-800',
  warning: 'bg-amber-50 border-amber-200 text-amber-800',
  error: 'bg-red-50 border-red-200 text-red-800',
};

export function Alert({ children, variant = 'info', className = '' }) {
  return (
    <div className={`rounded-md border p-4 text-sm ${variants[variant]} ${className}`}>
      {children}
    </div>
  );
}
