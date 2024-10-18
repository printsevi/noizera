import { forwardRef } from 'react';
import { twMerge } from 'tailwind-merge';
import { Button } from './ui/button';

export interface ButtonProps
  extends React.ButtonHTMLAttributes<HTMLButtonElement> {}

const PurpleButton = forwardRef<HTMLButtonElement, ButtonProps>(
  ({ className, children, disabled, ...props }, ref) => {
    return (
      <Button
        className={twMerge(
          `
        w-full 
        rounded-full 
        bg-purple-500
        hover:bg-purple-500
        border
        border-transparent
        px-3 
        py-3 
        disabled:cursor-not-allowed 
        disabled:opacity-50
        text-black
        font-bold
        hover:opacity-75
        transition
      `,
          disabled && 'opacity-75 cursor-not-allowed',
          className
        )}
        disabled={disabled}
        ref={ref}
        {...props}
      >
        {children}
      </Button>
    );
  }
);

PurpleButton.displayName = 'PurpleButton';

export default PurpleButton;
