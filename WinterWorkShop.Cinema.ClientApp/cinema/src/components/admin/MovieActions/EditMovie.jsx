import React from 'react';
import { withRouter } from 'react-router-dom';
import { FormGroup, FormControl, Button, Container, Row, Col, FormText, } from 'react-bootstrap';
import { NotificationManager } from 'react-notifications';
import { serviceConfig } from '../../../appSettings';
import { YearPicker } from 'react-dropdown-date';
import Switch from "react-switch";
import ReactStars from 'react-stars';

const ratingChanged = (newRating) => {
    console.log(newRating)
}

class EditMovie extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            title: '',
            year: 0,
            rating: '',
            id: '',
            current: false,
            titleError: '',
            yearError: '',
            submitted: false,
            canSubmit: true
        };
        this.change = this.change.bind(this);
        this.handleSubmit = this.handleSubmit.bind(this);
        this.handleYearChange = this.handleYearChange.bind(this);
    }

    componentDidMount() {
        const { id } = this.props.match.params;
        this.getMovie(id);
    }

    change(e) {
        const { id, value } = e.target;
        this.setState({ [id]: value });
        this.validate(id, value);
    }

    validate(id, value) {
        if (id === 'title') {
            if (value === '') {
                this.setState({
                    titleError: 'Fill in movie title',
                    canSubmit: false
                });
            } else {
                this.setState({
                    titleError: '',
                    canSubmit: true
                });
            }
        }

        if (id === 'year') {
            const yearNum = +value;
            if (!value || value === '' || (yearNum < 1895 || yearNum > 2100)) {
                this.setState({
                    yearError: 'Please chose valid year',
                    canSubmit: false
                });
            } else {
                this.setState({
                    yearError: '',
                    canSubmit: true
                });
            }
        }
    }

    handleSubmit(e) {
        e.preventDefault();

        this.setState({ submitted: true });
        const { title, year, rating } = this.state;
        if (title && year && rating) {
            this.updateMovie();
        } else {
            NotificationManager.error('Please fill in data');
            this.setState({ submitted: false });
        }
    }

    handleYearChange(year) {
        this.setState({ year: year });
        this.validate('year', year);
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

    updateMovie() {
        const { title, year, current, rating, id } = this.state;

        const data = {
            Title: title,
            Year: +year,
            Current: current === 'true',
            Rating: +rating
        };

        const requestOptions = {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': 'Bearer ' + localStorage.getItem('jwt')
            },
            body: JSON.stringify(data)
        };

        fetch(`${serviceConfig.baseURL}/api/movies/${id}`, requestOptions)
            .then(response => {
                if (!response.ok) {
                    return Promise.reject(response);
                }
                return response.statusText;
            })
            .then(result => {
                this.props.history.goBack();
                NotificationManager.success('Successfuly edited movie!');
            })
            .catch(response => {
                NotificationManager.error(response.message || response.statusText);
                this.setState({ submitted: false });
            });
    }


    render() {
        const { title, year, current, rating, submitted, titleError, yearError, canSubmit } = this.state;
        return (
            <Container>
                <Row>
                    <Col>
                        <h1 className="form-header">Edit Existing Movie</h1>
                        <form onSubmit={this.handleSubmit}>
                            <FormGroup>
                                <FormControl
                                    id="title"
                                    type="text"
                                    placeholder="Movie Title"
                                    value={title}
                                    onChange={this.change}
                                />
                                <FormText className="text-danger">{titleError}</FormText>
                            </FormGroup>
                            <FormGroup>
                                <YearPicker
                                    defaultValue={'Select Movie Year'}
                                    start={1895}
                                    end={2120}
                                    reverse
                                    required={true}
                                    disabled={false}
                                    value={year}
                                    onChange={(year) => {
                                        this.handleYearChange(year);
                                    }}
                                    id={'year'}
                                    name={'year'}
                                    classes={'form-control'}
                                    optionClasses={'option classes'}
                                />
                                <FormText className="text-danger">{yearError}</FormText>
                            </FormGroup>
                            <FormGroup>
                                <FormControl as="select" placeholder="Rating" id="rating" value={rating} onChange={this.change}>
                                    <option value="1">1</option>
                                    <option value="2">2</option>
                                    <option value="3">3</option>
                                    <option value="4">4</option>
                                    <option value="5">5</option>
                                    <option value="6">6</option>
                                    <option value="7">7</option>
                                    <option value="8">8</option>
                                    <option value="9">9</option>
                                    <option value="10">10</option>
                                </FormControl>
                            </FormGroup>
                            <FormGroup>
                            <td className="text-center cursor-pointer">{<ReactStars count={10} onChange={ratingChanged} edit = {false} size={37} value={rating} color1 = {'grey'} color2={'#ffd700'} />}</td>
                            </FormGroup>
                            <FormGroup>
                                <FormControl as="select" placeholder="Current" id="current" value={current} onChange={this.change}>
                                    <option value="true">Current</option>
                                    <option value="false">Not Current</option>
                                </FormControl>
                            </FormGroup>
                            <FormGroup>
                                <td className >{current === 'true' ? <Switch onChange={this.handleChange} checked={true} /> : <Switch onChange={this.handleChange} checked={false} />}      </td>
                            </FormGroup>
                            <Button type="submit" disabled={submitted || !canSubmit} block>Edit Movie</Button>
                        </form>
                    </Col>
                </Row>
            </Container>
        );
    }
}
export default withRouter(EditMovie);