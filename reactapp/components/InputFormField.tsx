import cn from 'classnames'
import { Control, FieldValues } from 'react-hook-form'
import { Input } from './ui/input'
import { FormControl, FormDescription, FormField, FormItem, FormLabel, FormMessage } from './ui/form'

interface Props {
    name: string,
    label?: string,
    type?: any,
    id?: string,
    placeholder?: string,
    className?: string,
    disabled?: boolean,
    onChange?: (v: string | undefined) => void,
    control?: Control<FieldValues>,
    description?: string
}

export const InputFormField = (props: Props) => {
  return (
    <FormField
      control={props.control}
      name={props.name}
      render={({ field }) => (
        <FormItem>
          <FormLabel>{props.label}</FormLabel>
          <FormControl>
            <Input 
              id={props.id}
              type={props.type}
              className={cn(props.disabled && 'opacity-75')}
              placeholder={props.placeholder}
              disabled={props.disabled}
              maxLength={128} 
              {...field} 
              onChange={(e) => {
                field.onChange(e);
                if(props.onChange) {
                  props.onChange(e?.target?.value)
                }
              }}
              value={field.value}
            />
          </FormControl>
          <FormDescription>
            {props.description}
          </FormDescription>
          <FormMessage />
        </FormItem>
      )}
    />
  );
}