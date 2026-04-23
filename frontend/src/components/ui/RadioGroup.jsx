export function RadioGroup({ name, value, onChange, children }) {
  return (
    <div className="space-y-2" role="radiogroup">
      {children({ name, value, onChange })}
    </div>
  );
}

export function RadioOption({ name, value, checked, onChange, label, description }) {
  const id = `${name}-${value}`;

  return (
    <label
      htmlFor={id}
      className={`flex items-center gap-3 rounded-lg border p-4 cursor-pointer transition-colors
        hover:bg-gray-50
        ${checked ? 'border-[var(--color-primary)] bg-[var(--color-primary)]/5' : 'border-[var(--color-border)]'}`}
    >
      <input
        type="radio"
        id={id}
        name={name}
        value={value}
        checked={checked}
        onChange={() => onChange(value)}
        className="accent-[var(--color-primary)]"
      />
      <div>
        <p className="font-medium">{label}</p>
        {description && <p className="text-sm text-[var(--color-muted)]">{description}</p>}
      </div>
    </label>
  );
}
