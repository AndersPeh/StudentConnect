import {z} from 'zod';

const requiredString = (fieldName: string) => z
    .string({required_error: `${fieldName} is required`})

    .min(1, {message: `${fieldName} is required`});

// activitySchema defines the shape of the form's data, the form must have following keys.
export const activitySchema = z.object({

    // fieldName is only passed for constructing the error message. When title: requiredString, zod knows it is validating texfield named title.
    title: requiredString('Title'),
    description: requiredString('Description'),
    category: requiredString('Category'),
    date: z.coerce.date({message:'Date is required'}),
    city: requiredString('City'),
    venue: requiredString('Venue'),

})

// zod will create the ActivitySchema type for ActivityForm.
export type ActivitySchema = z.infer<typeof activitySchema>;