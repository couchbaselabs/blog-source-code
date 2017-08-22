// Views
//  patients bucket
//      _design/dev_patients design document
//          doctorPatientGroup view

// map
// tag::map[]
function (doc, meta) {
    emit([doc.doctorId], meta.id);
}
// end::map[]

// reduce
// tag::reduce[]
function reduce(key, values, rereduce) {
    var merged = [].concat.apply([], values);
    return merged;
}
// end::reduce[]

// http://127.0.0.1:8092/patients/_design/dev_patients/_view/doctorPatientGroup?connection_timeout=60000&full_set=true&group_level=1&inclusive_end=true&skip=0&stale=false


// example documents
// tag::exampledocs[]
key 01257721
{
    "doctorId": 58,
    "patientName": "Robyn Kirby",
    "patientDob": "1986-05-16T19:01:52.4075881-04:00"
}

key 116wmq8i
{
    "doctorId": 8,
    "patientName": "Helen Clark",
    "patientDob": "2016-02-01T04:54:30.3505879-05:00"
}
// end::exampledocs[]
