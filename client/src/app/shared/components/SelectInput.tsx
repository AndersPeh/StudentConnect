import {
  FormControl,
  FormHelperText,
  InputLabel,
  MenuItem,
  Select,
} from "@mui/material";
import type { SelectInputProps } from "@mui/material/Select/SelectInput";
import {
  useController,
  type UseControllerProps,
  type FieldValues,
} from "react-hook-form";

type Props<T extends FieldValues> = {
  items: { text: string; value: string }[];
  label: string;
} & UseControllerProps<T> &
  // Partial makes required properties optional.
  Partial<SelectInputProps>;

export default function SelectInput<T extends FieldValues>(props: Props<T>) {
  const { field, fieldState } = useController({ ...props });
  return (
    <FormControl fullWidth error={!!fieldState.error}>
      <InputLabel>{props.label}</InputLabel>
      <Select
        // field.value represents selected value by the user. In the beginning, it's empty.
        value={field.value || ""}
        label={props.label}
        // item.value go to here to reflect changes when user selects an option.
        onChange={field.onChange}
      >
        {props.items.map((item) => (
          // value is internal value that the Select component will use after user selects an option. {item.text} is option available to choose from.
          <MenuItem key={item.value} value={item.value}>
            {item.text}
          </MenuItem>
        ))}
      </Select>
      <FormHelperText>{fieldState.error?.message}</FormHelperText>
    </FormControl>
  );
}
