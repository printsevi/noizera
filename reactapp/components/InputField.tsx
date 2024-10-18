import cn from 'classnames'
import { AnimatePresence } from 'framer-motion'
import { InputError } from './InputError'

interface Props {
    label?: string,
    type?: any,
    id?: string,
    placeholder?: string,
    validation?: any,
    multiline?: boolean,
    className?: string,
    disabled?: boolean,
    value: string;
    onBlur?: (v: string | undefined) => void;
    onChange?: (v: string | undefined) => void
}

export const InputField = (props: Props) => {
  const isInvalid = false;

  const input_tailwind = 'flex w-full rounded-md bg-neutral-700 border border-transparent px-3 py-3 text-sm file:border-0 file:bg-transparent file:text-sm file:font-medium placeholder:text-neutral-400 disabled:cursor-not-allowed disabled:opacity-50 focus:outline-none';
  return (
    <div className={cn('flex flex-col w-full gap-2', props.className)}>
      <div className="flex justify-between">
        <label htmlFor={props.id}>
          {props.label}
        </label>
        <AnimatePresence mode="wait" initial={false}>
          {isInvalid && (
            <InputError
              message={"Error"}
            />
          )}
        </AnimatePresence>
      </div>
      <input
          id={props.id}
          type={props.type}
          className={cn(input_tailwind, props.disabled && 'opacity-75')}
          placeholder={props.placeholder}
          disabled={props.disabled}
          maxLength={500}
          onBlurCapture={(event) => props.onBlur ? props.onBlur(event.target.value) : {}}
          onChange={(event) => props.onChange ? props.onChange(event.target.value) : {}}
          value={props.value}
        />
    </div>
  )
}