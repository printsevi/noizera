import cn from 'classnames'
import { Control, FieldValues } from 'react-hook-form'
import { FormControl, FormDescription, FormField, FormItem, FormLabel, FormMessage } from './ui/form'
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from './ui/select'

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
    items: SelectFormFieldItem[]
}

interface SelectFormFieldItem {
  value: string,
  text: string
}

export const SelectFormField = (props: Props) => {
  return (
    <FormField
      control={props.control}
      name={props.name}
      render={({ field }) => (
        <FormItem>
          <FormLabel>{props.label}</FormLabel>
          <Select disabled={props.disabled} onValueChange={field.onChange} defaultValue={field.value}>
            <FormControl>
              <SelectTrigger>
                <SelectValue placeholder={props.placeholder} />
              </SelectTrigger>
            </FormControl>
            <SelectContent>
              {props.items.map((item) => {
                return (<SelectItem value={item.value}>{item.text}</SelectItem>)
              })}
            </SelectContent>
          </Select>
          <FormDescription>
            {props.description}
          </FormDescription>
          <FormMessage />
        </FormItem>
      )}
    />
  );
}