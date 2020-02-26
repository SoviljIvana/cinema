import React, { Component } from 'react';
import { NotificationManager } from 'react-notifications';
import { serviceConfig } from '../../appSettings';
import { FormGroup, FormControl, Button, Container, Row, Col, FormText, FormLabel, Alert } from 'react-bootstrap';
import { YearPicker } from 'react-dropdown-date';
import Switch from "react-switch";
import ReactStars from 'react-stars';

const ratingChanged = (newRating) => {
    console.log(newRating)
}

class ProjectionDetails extends Component {

    constructor(props) {
        super(props);
        this.state = {
            title: '',
            year: 0,
            rating: '',
            id: '',
            projectionTime: '',
            movieId: '',
            auditoriumId: '',
            current: false,
            titleError: '',
            yearError: '',
            submitted: false,
            canSubmit: true
        };

    }

    componentDidMount() {
        const { id, movieId } = this.props.match.params;
        this.getMovie(id);
        this.getProjections(movieId);
    }

    getProjections() {
        const projectionTimes = ['11:45', '12:25', '14:52', '17:30', '12:25', '14:52', '17:30', '12:25', '14:52', '17:30', '12:25', '14:52', '17:30', '12:25', '14:52', '17:30', '12:25', '14:52', '17:30'];
        return projectionTimes.map((time) => {
            return <Button className="mr-1 mb-2">{time}</Button>
        })
    }

    getMovie(movieId) {
        const requestOptions = {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': 'Bearer ' + localStorage.getItem('jwt')
            }
        };

        fetch(`${serviceConfig.baseURL}/api/movies/` + movieId, requestOptions)
            .then(response => {
                if (!response.ok) {
                    return Promise.reject(response);
                }
                return response.json();
            })
            .then(data => {
                if (data) {
                    this.setState({
                        title: data.title,
                        year: data.year,
                        rating: Math.round(data.rating) + '',
                        current: data.current + '',
                        id: data.id
                    });
                }
            })
            .catch(response => {
                NotificationManager.error(response.message || response.statusText);
                this.setState({ submitted: false });
            });
    }

    render() {
        const { title, year,  rating,  titleError, yearError, projectionTime } = this.state;
        const projectionTimes = this.getProjections();
        return (
            <Container>
                <Row className="justify-content-center">
                    <Col>
                            <br></br>
                            <FormGroup>
                                <FormControl
                                    id="title"
                                    type="text"
                                    placeholder="Movie Title"
                                    value={title}
                                />
                                <FormText className="text-danger">{titleError}</FormText>
                            </FormGroup>
                            <FormGroup>
                                <FormControl
                                    defaultValue={'Select Movie Year'}
                                    start={1895}
                                    end={2120}
                                    reverse
                                    required={true}
                                    disabled={false}
                                    value={year}
                                    id={'year'}
                                    name={'year'}
                                    classes={'form-control'}
                                    optionClasses={'option classes'}
                                />
                                
                                <FormText className="text-danger">{yearError}</FormText>
                            </FormGroup>
                            <FormGroup>
                                <td className="text-center cursor-pointer">{<ReactStars count={10} edit={false} size={37} value={rating} color1={'grey'} color2={'#ffd700'} />}</td>
                            </FormGroup>
                            <FormGroup>
                                {projectionTimes}
                            </FormGroup>
                    </Col>
                </Row>
            </Container>
        );
    }
}
export default ProjectionDetails;
