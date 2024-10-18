import { Control, FieldValues } from 'react-hook-form'
import { FormControl, FormDescription, FormField, FormItem, FormLabel, FormMessage } from './ui/form'
import { Checkbox } from './ui/checkbox'
import { Label } from './ui/label'

interface Props {
    name: string,
    label?: string,
    id?: string,
    placeholder?: string,
    className?: string,
    disabled?: boolean,
    onChange?: (v: string | undefined) => void,
    control?: Control<FieldValues>,
    description?: string
}

export const CheckboxFormField = (props: Props) => {
  return (
    <FormField
      control={props.control}
      name={props.name}
      render={({ field }) => (
        <FormItem>
          <FormControl>
            <Checkbox
              disabled={props.disabled} 
              checked={field.value}
              onCheckedChange={field.onChange}
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