import { TextField, type TextFieldProps } from "@mui/material";
import {
  useController,
  type UseControllerProps,
  type FieldValues,
} from "react-hook-form";

// & is an intersection type operator, it means an object of type Props must have all required properties
// from UseControllerProps (name, control) and TextFieldProps (no required property).
// control and name props belong to UseControllerProps, label prop belongs to TextFieldProps.
// Because control provided is <ActivitySchema> type, <T> automatically becomes <ActivitySchema> type.
// Because of <T extends FieldValues>, <T> becomes <ActivitySchema> object where keys are strings and any values.
// For example, name="title". name becomes key and this key has to match one of the keys in activitySchema.
type Props<T extends FieldValues> = {} & UseControllerProps<T> & TextFieldProps;

// FieldValues is a generic type where keys are strings (field names) and values can be anything.
// <T> must be an object that has string keys with any values. Props<T> refers to the <T> from TextInput.
export default function TextInput<T extends FieldValues>(props: Props<T>) {
  // useController takes properties it needs from props, then returns fieldState and field.
  // It takes name to know which field of the form it is managing, it also takes control to have information about value, onChange, onBlur, error etc.
  // fieldState consists of error, isTouched etc..
  const { field, fieldState } = useController({ ...props });
  return (
    <TextField
      // for example, label="Description", defaultValue={activity?.description}, multiline, rows={3}
      {...props}
      // field from useController({ ...props }) consists of onChange (update value when user makes change in the field), onBlur (when user tabs something else),
      // component value (shows current value as user types), input name etc... for Textfield to know how to interact with user click and input.
      {...field}
      // Props come later will override earlier ones with the same name, this field.value will set value and override field.value from above.
      value={field.value || ""}
      fullWidth
      variant="outlined"
      // After zodResolver validates the field value, if it doesnt match ActivitySchema, it returns error object to here.
      // highlights the border of textfield red when the fieldState exists in errors object.
      error={!!fieldState.error}
      // displays validation message from activitySchema.
      helperText={fieldState.error?.message}
    />
  );
}
