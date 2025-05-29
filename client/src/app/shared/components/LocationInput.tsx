import { useEffect, useMemo, useState } from "react";
import {
  useController,
  type FieldValues,
  type UseControllerProps,
} from "react-hook-form";
import { type LocationIQSuggestion } from "../../../lib/types";
import {
  Box,
  debounce,
  List,
  ListItemButton,
  TextField,
  Typography,
} from "@mui/material";
import axios from "axios";

// Props connects to generic key (string) value pair and passes it to UseControllerProps,
// any property in UseControllerProps<T> that has <T> type knows its structure is <T>.
// Props must have:
// a property name label with string value.
// props required by useController.
type Props<T extends FieldValues> = { label: string } & UseControllerProps<T>;
// In ActivityForm, props looks like this:
// <LocationInput<ActivitySchema> // T for LocationInput is ActivitySchema
//   label="Enter the location"    // Prop 1
//   name="location"               // Prop 2 (for useController)
//   control={control}            // Prop 3 (for useController)
// />

// props is LocationInput with ActivitySchema as T, then the T is passed to control,
// so control knows it is managing a form with the structure ActivitySchema.
export default function LocationInput<T extends FieldValues>(props: Props<T>) {
  // useController takes props after spreading and returns field and fieldState
  // for LocationInput to interact with location field in react form.
  const { field, fieldState } = useController({ ...props });
  const [loading, setLoading] = useState(false);
  const [suggestions, setSuggestions] = useState<LocationIQSuggestion[]>([]);
  const [inputValue, setInputValue] = useState(field.value || "");

  useEffect(() => {
    // When field.value is an object (loaded from existing activity), it contains location object, only need to display full address, so extract venue.
    if (field.value && typeof field.value === "object") {
      setInputValue(field.value.venue || "");
    } else {
      // When user types, field.value is just a string, input value changes as field.value is changed by the user.
      setInputValue(field.value || "");
    }
  }, [field.value]);

  const locationUrl = `https://api.locationiq.com/v1/autocomplete?key=pk.70918813d1ba57a226746af70792b478&limit=5&dedupe=1&`;

  // Because locationUrl is a const, useMemo only keeps a single instance of debounced function across re-renders.
  // When user changes the query, useMemo executes the same debounce function, debounce cancels previous operation, uses new query and restarts timer.
  // Without useMemo, whenever the component re-render, a new debounce function will be created without cancelling the old debounce function.
  // User will see weird results related to previous query because multiple debounce functions are running instead of a single debounce function.
  const fetchSuggestions = useMemo(
    () =>
      // debounce delays the inner function execution by 0.5s (send get request).
      // during the 0.5s wait, if there is a new query, it will restart the timer with the new message.
      // if there is no new message after 0.5s, it will send the get request.
      debounce(async (query: string) => {
        if (!query || query.length < 3) {
          setSuggestions([]);
          return;
        }
        setLoading(true);
        try {
          const respond = await axios.get<LocationIQSuggestion[]>(
            `${locationUrl}q=${query}`
          );
          setSuggestions(respond.data);
        } catch (error) {
          console.log(error);
        } finally {
          setLoading(false);
        }
      }, 500),
    [locationUrl]
  );

  const handleChange = async (value: string) => {
    // reflects changes to display what user types.
    field.onChange(value);
    await fetchSuggestions(value);
  };

  const handleSelect = (location: LocationIQSuggestion) => {
    const city =
      location.address?.city ||
      location.address?.town ||
      location.address?.village;

    const venue = location.display_name;

    const latitude = location.lat;

    const longitude = location.lon;

    // As suggestion.display_name shows full address, display the full adddress which is venue when user selects an option to maintain consistency.
    setInputValue(venue);
    // when user selects an option provided, updates the form city, venue, latitude and longtude of location object in ActivityForm.
    field.onChange({ city, venue, latitude, longitude });
    setSuggestions([]);
  };

  return (
    <Box>
      <TextField
        {...props}
        value={inputValue}
        onChange={(event) => handleChange(event.target.value)}
        fullWidth
        variant="outlined"
        error={!!fieldState.error}
        helperText={fieldState.error?.message}
      />
      {loading && <Typography>Loading...</Typography>}
      {suggestions.length > 0 && (
        <List sx={{ border: 1 }}>
          {suggestions.map((suggestion) => (
            <ListItemButton
              divider
              key={suggestion.place_id}
              onClick={() => handleSelect(suggestion)}
            >
              {/* display_name also shows full address like venue. */}
              {suggestion.display_name}
            </ListItemButton>
          ))}
        </List>
      )}
    </Box>
  );
}
